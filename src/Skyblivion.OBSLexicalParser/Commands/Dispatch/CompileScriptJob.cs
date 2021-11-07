using Skyblivion.OBSLexicalParser.Builds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class CompileScriptJob
    {
        private readonly IReadOnlyList<BuildTargetSimple> buildTargets;
        private readonly string workspacePath;
        private readonly string standardOutputFilePath, standardErrorFilePath;
        public CompileScriptJob(Build build, IReadOnlyList<BuildTargetSimple> buildTargets)
        {
            this.workspacePath = build.GetWorkspacePath();
            this.standardOutputFilePath = build.GetCompileStandardOutputPath();
            this.standardErrorFilePath = build.GetCompileStandardErrorPath();
            this.buildTargets = buildTargets;
        }

        public void Run()
        {
            //Delete old output files
            File.Delete(standardOutputFilePath);
            File.Delete(standardErrorFilePath);
            int targetNumber = 0;
            foreach (BuildTargetSimple buildTarget in buildTargets)
            {
                string targetName = buildTarget.Name;
                targetNumber++;
                Console.WriteLine("Compiling Target " + targetName + " (" + targetNumber + "/" + buildTargets.Count + "):");
                buildTarget.Compile(buildTarget.GetTranspiledPath(), workspacePath, buildTarget.GetArtifactsPath(), standardOutputFilePath, standardErrorFilePath);
                Console.WriteLine("Compiling Target " + targetName + " (" + targetNumber + "/" + buildTargets.Count + ") Complete");
            }
            Console.WriteLine("Compiling Targets Complete");
        }
    }
}