using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class PrepareWorkspaceJob
    {
        public const int CopyOperationsPerBuildTarget = 2;
        private BuildTargetCollection buildTargetCollection;
        public PrepareWorkspaceJob(BuildTargetCollection buildTargetCollection)
        {
            this.buildTargetCollection = buildTargetCollection;
        }

        public void run(ProgressWriter progressWriter)
        {
            foreach (var buildTarget in buildTargetCollection)
            {
                string workspacePath = buildTarget.getWorkspacePath();
                FileTransfer.CopyDirectoryFiles(buildTarget.getTranspiledPath(), workspacePath, false);
                progressWriter.IncrementAndWrite();
                FileTransfer.CopyDirectoryFiles(buildTarget.getDependenciesPath(), workspacePath, true);//WTM:  Note:  Dependencies often (or possibly always) conflict.
                progressWriter.IncrementAndWrite();
            }
        }
    }
}