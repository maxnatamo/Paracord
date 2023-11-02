using System.Collections.Generic;
using System.IO;
using System.Linq;

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;

partial class Build : NukeBuild
{
    private static readonly AbsolutePath[] Projects =
    {
        RootDirectory / "src" / "Core",
        RootDirectory / "src" / "Shared",
        RootDirectory / "src" / "Templates",
    };

    private static IEnumerable<string> GetAllProjects()
    {
        foreach(AbsolutePath projectDirectory in Projects)
        {
            foreach(string projectFile in Directory.EnumerateFiles(projectDirectory, "*.csproj", SearchOption.AllDirectories))
            {
                yield return projectFile;
            }
        }
    }

    private void GenerateSolutionFile()
    {
        if(File.Exists(MainSolutionFile))
        {
            File.Delete(MainSolutionFile);
        }

        string workingDirectory = Path.GetDirectoryName(MainSolutionFile);
        string projects = string.Join(" ", Build.GetAllProjects().Select(p => $"\"{p}\""));

        DotNetTasks.DotNet($"new sln -n {Path.GetFileNameWithoutExtension(MainSolutionFile)}", workingDirectory);
        DotNetTasks.DotNet($"sln {MainSolutionFile} add {projects}", workingDirectory);
    }
}