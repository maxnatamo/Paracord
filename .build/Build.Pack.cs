using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild
{
    [Parameter("Name of the NuGet source")]
    readonly string NuGetSourceName = "gitlab";

    [Parameter("NuGet Source for packages")]
    readonly string NuGetSource;

    [Parameter("NuGet username")]
    readonly string NuGetUsername;

    [Parameter("NuGet password")]
    readonly string NuGetPassword;

    Target Pack => _ => _
        .DependsOn(Compile)
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
        .DependsOn(Compile)
        .Consumes(Pack)
        .Requires(() => NuGetSourceName)
        .Requires(() => NuGetSource)
        .Requires(() => NuGetUsername)
        .Requires(() => NuGetPassword)
        .Requires(() => Configuration.IsRelease)
        .Executes(() =>
        {
            var packages = NuGetArtifactsDirectory.GlobFiles("*.nupkg");

            DotNetNuGetAddSource(c => c
                .SetName(NuGetSourceName)
                .SetSource(NuGetSource)
                .SetUsername(NuGetUsername)
                .SetPassword(NuGetPassword)
                .SetStorePasswordInClearText(true));

            DotNetNuGetPush(c => c
                .SetSource(NuGetSource)
                .CombineWith(packages, (s, v) => s.SetTargetPath(v)));
        });
}