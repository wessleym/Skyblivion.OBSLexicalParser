using Skyblivion.OBSLexicalParser.Builds;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class CompileScriptJob
    {
        private BuildTargetCollection buildTargetCollection;
        private string logPath;
        public CompileScriptJob(BuildTargetCollection buildTargetCollection, string logPath)
        {
            this.buildTargetCollection = buildTargetCollection;
            this.logPath = logPath;
        }

        public void run()
        {
            foreach (var buildTarget in buildTargetCollection)
            {
                string[] contents = buildTarget.compile(buildTarget.getTranspiledPath(), buildTarget.getWorkspacePath(), buildTarget.getArtifactsPath());
                File.WriteAllLines(logPath, contents);
            }
        }
    }
}