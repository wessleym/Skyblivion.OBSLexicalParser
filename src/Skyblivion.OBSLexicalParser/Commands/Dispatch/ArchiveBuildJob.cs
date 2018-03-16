using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class ArchiveBuildJob
    {
        private string buildTarget;
        public ArchiveBuildJob(string buildTarget)
        {
            this.buildTarget = buildTarget;
        }

        public void run()
        {
            Build build = null;//WTM:  Change:  BuildTargetFactory.get takes two arguments, but in PHP, it was invoked with one argument.
            BuildTarget buildTarget = BuildTargetFactory.get(this.buildTarget, build);
            int latestBuild = Directory.EnumerateFileSystemEntries(buildTarget.getArchivePath())
                .Select(path => Path.GetFileName(path))
                .Select(name =>
                {
                    int fileBuild;
                    return int.TryParse(name, out fileBuild) ? fileBuild : 0;
                })
                .Max();
            int archivedBuild = latestBuild + 1;
            Directory.CreateDirectory(buildTarget.getArchivedBuildPath(archivedBuild));
            //WTM:  Change:  BuildTargetFactory.get takes two arguments, but in PHP, it was invoked with one argument.
            string sourcePath = null;//buildTarget.getBuildPath()
            string destinationPath = buildTarget.getArchivedBuildPath(archivedBuild);
            FileTransfer.CopyDirectoryFiles(sourcePath, destinationPath);

            Process.Start("./clean.sh", this.buildTarget);
        }
    }
}