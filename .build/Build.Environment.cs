using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.GitVersion;

partial class Build : NukeBuild
{
    /// <summary>
    /// Path to the main solution file of the project.
    /// </summary>
    readonly AbsolutePath MainSolutionFile = RootDirectory / "Paracord.sln";

    /// <summary>
    /// Path to store coverage reports.
    /// </summary>
    readonly AbsolutePath TestCoverageDirectory = RootDirectory / "coverage";

    /// <summary>
    /// Path to store local artifacts.
    /// </summary>
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";

    [GitVersion]
    readonly GitVersion GitVersion;

    [GitRepository]
    readonly GitRepository GitRepository;
}