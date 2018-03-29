using Skyblivion.OBSLexicalParser.Builds;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class CompileScriptJob
    {
        private BuildTargetCollection buildTargetCollection;
        private string standardOutputFilePath, standardErrorFilePath;
        public CompileScriptJob(Build build, BuildTargetCollection buildTargetCollection)
        {
            this.standardOutputFilePath = build.getCompileStandardOutputPath();
            this.standardErrorFilePath = build.getCompileStandardErrorPath();
            this.buildTargetCollection = buildTargetCollection;
        }

        public void run()
        {
            BuildTarget[] targets = buildTargetCollection.ToArray();
            int targetNumber = 0;
            foreach (BuildTarget buildTarget in targets)
            {
                string targetName = buildTarget.getTargetName();
                targetNumber++;
                Console.WriteLine("Compiling Target " + targetName + " (" + targetNumber + "/" + targets.Length + "):");
                buildTarget.compile(buildTarget.getTranspiledPath(), buildTarget.getWorkspacePath(), buildTarget.getArtifactsPath(), standardOutputFilePath, standardErrorFilePath);
                Console.WriteLine("Compiling Target " + targetName + " (" + targetNumber + "/" + targets.Length + ") Complete");
            }
            Console.WriteLine("Compiling Targets Complete");
        }
    }
}