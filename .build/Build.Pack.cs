using System.Collections.Generic;

using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild
{
    [Parameter("NuGet package endpoint for nuget.org")]
    readonly string NugetSource = "https://api.nuget.org/v3/index.json";

    [Secret]
    [Parameter("NuGet API key for authorization for nuget.org")]
    readonly string NugetApiKey;

    private IReadOnlyCollection<AbsolutePath> NugetArtifacts;

    Target Pack => _ => _
        .DependsOn(Compile, Format, Test)
        .Produces(NuGetArtifactsDirectory / "*.nupkg")
        .Produces(NuGetArtifactsDirectory / "*.snupkg")
        .Requires(() => Configuration.IsRelease)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(MainSolutionFile)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetIncludeSymbols(true)
                .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
                .SetIncludeSource(false)
                .SetDescription("Homebrew HTTP server for .NET Core")
                .SetAuthors("Max T. Kristiansen")
                .SetCopyright("Copyright (c) Max T. Kristiansen 2023")
                .SetPackageTags("http https web-server c# core library")
                .SetPackageProjectUrl("https://github.com/maxnatamo/paracord")
                .SetNoDependencies(false)
                .SetOutputDirectory(NuGetArtifactsDirectory)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetVersion(GitVersion.SemVer));
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Triggers(PublishNuget)
        .Triggers(PublishGitHub)
        .Executes(() =>
        {
            NugetArtifacts = NuGetArtifactsDirectory.GlobFiles("*.nupkg");
        });

    Target PublishNuget => _ => _
        .DependsOn(Publish)
        .Requires(() => NugetSource)
        .Requires(() => NugetApiKey)
        .Requires(() => Configuration.IsRelease)
        .OnlyWhenDynamic(() => NugetArtifacts.Count > 0)
        .Executes(() =>
        {
            foreach(AbsolutePath package in NugetArtifacts)
            {
                DotNetNuGetPush(c => c
                    .SetTargetPath(package)
                    .SetSource(NugetSource)
                    .SetApiKey(NugetApiKey)
                    .EnableSkipDuplicate());
            }
        });

    Target PublishGitHub => _ => _
        .DependsOn(Publish)
        .Requires(() => NugetSource)
        .Requires(() => NugetApiKey)
        .Requires(() => Configuration.IsRelease)
        .OnlyWhenStatic(() => Host is GitHubActions)
        .OnlyWhenDynamic(() => NugetArtifacts.Count > 0)
        .Executes(() =>
        {
            GitHubActions Instance = (GitHubActions) Host;

            foreach(AbsolutePath package in NugetArtifacts)
            {
                DotNetNuGetPush(c => c
                    .SetTargetPath(package)
                    .SetSource($"https://nuget.pkg.github.com/{Instance.RepositoryOwner}/index.json")
                    .SetApiKey(Instance.Token)
                    .EnableSkipDuplicate());
            }
        });
}