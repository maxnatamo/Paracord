using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitLab;

[GitHubActions(
    "merge-request",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPushBranches = new[]
    {
        "main"
    },
    OnPullRequestBranches = new[]
    {
        "main"
    },
    InvokedTargets = new[]
    {
        nameof(Format),
        nameof(Test)
    }
)]
partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild
        ? Configuration.Debug
        : Configuration.Release;

    protected override void OnBuildInitialized()
    {
        Serilog.Log.Information("🪢 Build process started");
        Serilog.Log.Information("");
        Serilog.Log.Information("Build manifest:");
        Serilog.Log.Information("  Git branch: {BranchName}", GitVersion.BranchName);
        Serilog.Log.Information("  Git commit hash: {ShortSha}", GitVersion.ShortSha);
        Serilog.Log.Information("  Git semantic version: {SemVer}", GitVersion.SemVer);

        if(Host is GitLab)
        {
            GitLab GitLab = Host as GitLab;

            Serilog.Log.Information("  GitLab Job ID: {JobId}", GitLab.JobId);
            Serilog.Log.Information("  Triggered by: {Name} ({Username})", GitLab.GitLabUserName, GitLab.GitLabUserLogin);
        }

        if(Host is GitHubActions)
        {
            GitHubActions GitHub = Host as GitHubActions;

            Serilog.Log.Information("  GitHub Job ID: {JobId}", GitHub.JobId);
            Serilog.Log.Information("  Triggered by: {Actor}", GitHub.Actor);
        }

        base.OnBuildInitialized();
    }
}