using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class PrepareWorkspaceJob
    {
        private BuildTargetCollection buildTargetCollection;
        public PrepareWorkspaceJob(BuildTargetCollection buildTargetCollection)
        {
            this.buildTargetCollection = buildTargetCollection;
        }

        public void run()
        {
            foreach (var buildTarget in buildTargetCollection)
            {
                FileTransfer.CopyDirectoryFiles(buildTarget.getTranspiledPath(), buildTarget.getWorkspacePath());
                FileTransfer.CopyDirectoryFiles(buildTarget.getDependenciesPath(), buildTarget.getWorkspacePath());
            }
        }
    }
}