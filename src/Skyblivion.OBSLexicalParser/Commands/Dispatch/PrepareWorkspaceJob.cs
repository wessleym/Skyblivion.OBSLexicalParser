using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class PrepareWorkspaceJob
    {
        public const int CopyOperationsPerBuildTarget = 2;
        private readonly IEnumerable<BuildTarget> buildTargets;
        public PrepareWorkspaceJob(IEnumerable<BuildTarget> buildTargets)
        {
            this.buildTargets = buildTargets;
        }

        public void Run(ProgressWriter progressWriter)
        {
            foreach (var buildTarget in buildTargets)
            {
                string workspacePath = buildTarget.GetWorkspacePath();
                FileTransfer.CopyDirectoryFiles(buildTarget.GetTranspiledPath(), workspacePath, false);
                progressWriter.IncrementAndWrite();
                FileTransfer.CopyDirectoryFiles(buildTarget.GetDependenciesPath(), workspacePath, true);//WTM:  Note:  Dependencies often (or possibly always) overwrite each other.
                progressWriter.IncrementAndWrite();
            }
        }
    }
}