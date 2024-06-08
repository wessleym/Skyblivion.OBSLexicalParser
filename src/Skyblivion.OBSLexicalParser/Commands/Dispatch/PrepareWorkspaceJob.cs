using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class PrepareWorkspaceJob
    {
        public const int CopyOperationsPerBuildTarget = 1/*2*/;
        private readonly Build build;
        private readonly IEnumerable<BuildTarget> buildTargets;
        public PrepareWorkspaceJob(Build build, IEnumerable<BuildTarget> buildTargets)
        {
            this.build = build;
            this.buildTargets = buildTargets;
        }

        public void Run(ProgressWriter progressWriter)
        {
            string dependenciesPath = build.GetDependenciesPath();
            string workspacePath = build.GetWorkspacePath();
            FileWriter.CopyDirectoryFilesRecursive(dependenciesPath, workspacePath, false);
            progressWriter.IncrementAndWrite();
            foreach (var buildTarget in buildTargets)
            {
                FileWriter.CopyDirectoryFilesRecursive(buildTarget.GetTranspiledPath(), workspacePath, false);
                progressWriter.IncrementAndWrite();
                //WTM:  Change:  I'm now using one dependencies folder instead of three.
                //FileTransfer.CopyDirectoryFiles(buildTarget.GetDependenciesPath(), workspacePath, true);//WTM:  Note:  Dependencies often (or possibly always) overwrite each other.
                //progressWriter.IncrementAndWrite();
            }
        }
    }
}