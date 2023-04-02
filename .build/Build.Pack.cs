using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild
{
    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(ArtifactsDirectory / "*.nupkg")
        .Produces(ArtifactsDirectory / "*.snupkg")
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
                .SetNoDependencies(true)
                .SetOutputDirectory(ArtifactsDirectory / "nuget")
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetVersion(GitVersion.SemVer));
        });
}