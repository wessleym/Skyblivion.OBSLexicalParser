using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Graph;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class TranspileScriptJob
    {
        private BuildTargetCollection buildTargets;
        private string scriptName;
        /*
        * TranspileScriptJob constructor.
        */
        public TranspileScriptJob(BuildTargetCollection buildTargets, string scriptName)
        {
            this.buildTargets = buildTargets;
            this.scriptName = scriptName;
        }

        public void run(TES5GlobalScope globalScope, TES5MultipleScriptsScope compilingScope)
        {//WTM:  Change:  Below, buildTarget.transpile requires TES5GlobalScope and TES5MultipleScriptsScope.  I added them above.
            string[] scripts = this.buildTargets.getScriptsToCompile(this.scriptName);
            BuildSourceFilesCollection partitionedScripts = this.buildTargets.getSourceFiles(scripts);
            List<string> sourcePaths = new List<string>();
            List<string> outputPaths = new List<string>();
            var buildTarget = buildTargets.First();//WTM:  Change:  buildTarget was undefined in PHP.
            foreach (var buildScript in scripts)
            {
                string scriptName = Path.GetFileName(buildScript);
                string sourcePath = buildTarget.getSourceFromPath(scriptName);
                string outputPath = buildTarget.getTranspileToPath(scriptName);
                sourcePaths.Add(sourcePath);
                outputPaths.Add(outputPath);
                buildTarget.transpile(sourcePath, outputPath, globalScope, compilingScope);
                //WTM:  Change:  The above line was moved from below (outside loop).
            }
            //buildTarget.transpile(sourcePaths, outputPaths, globalScope, compilingScope);
        }
    }
}