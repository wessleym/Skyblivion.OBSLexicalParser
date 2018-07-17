using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class ArchiveBuildJob
    {
        private readonly string buildTarget;
        public ArchiveBuildJob(string buildTarget)
        {
            this.buildTarget = buildTarget;
        }

        public void Run()
        {
            Build build = null;//WTM:  Change:  BuildTargetFactory.get takes two arguments, but in PHP, it was invoked with one argument.  This file will fail to run.
            using (BuildLogServices buildLogServices = new BuildLogServices(build))
            {
                BuildTarget buildTarget = BuildTargetFactory.Get(this.buildTarget, build, buildLogServices, true);
                int latestBuild = Directory.EnumerateFileSystemEntries(buildTarget.GetArchivePath())
                    .Select(path => Path.GetFileName(path))
                    .Select(name =>
                    {
                        int fileBuild;
                        return int.TryParse(name, out fileBuild) ? fileBuild : 0;
                    })
                    .Max();
                int archivedBuild = latestBuild + 1;
                Directory.CreateDirectory(buildTarget.GetArchivedBuildPath(archivedBuild));
                //WTM:  Change:  buildTarget.getBuildPath() is not a valid method.
                string sourcePath = null;//buildTarget.getBuildPath()
                string destinationPath = buildTarget.GetArchivedBuildPath(archivedBuild);
                FileTransfer.CopyDirectoryFiles(sourcePath, destinationPath, false);
            }
            Process.Start("clean.sh", this.buildTarget);
        }
    }
}