using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build : NukeBuild
{
    Target Restore => _ => _
        .Executes(() =>
        {
            this.GenerateSolutionFile();

            DotNetRestore(c => c
                .SetProjectFile(MainSolutionFile));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            if(!InvokedTargets.Contains(Restore))
            {
                this.GenerateSolutionFile();
            }

            DotNetBuild(c => c
                .SetProjectFile(MainSolutionFile)
                .SetNoRestore(InvokedTargets.Contains(Restore))
                .SetConfiguration(Configuration));
        });
}