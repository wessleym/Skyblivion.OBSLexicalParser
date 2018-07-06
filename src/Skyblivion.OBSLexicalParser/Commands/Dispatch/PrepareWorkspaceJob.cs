using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class PrepareWorkspaceJob
    {
        public const int CopyOperationsPerBuildTarget = 2;
        private readonly BuildTargetCollection buildTargetCollection;
        public PrepareWorkspaceJob(BuildTargetCollection buildTargetCollection)
        {
            this.buildTargetCollection = buildTargetCollection;
        }

        public void Run(ProgressWriter progressWriter)
        {
            foreach (var buildTarget in buildTargetCollection)
            {
                string workspacePath = buildTarget.GetWorkspacePath();
                FileTransfer.CopyDirectoryFiles(buildTarget.GetTranspiledPath(), workspacePath, false);
                progressWriter.IncrementAndWrite();
                FileTransfer.CopyDirectoryFiles(buildTarget.GetDependenciesPath(), workspacePath, true);//WTM:  Note:  Dependencies often (or possibly always) conflict.
                progressWriter.IncrementAndWrite();
            }
        }
    }
}