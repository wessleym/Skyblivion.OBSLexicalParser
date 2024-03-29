using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class TranspileScriptJob
    {
        private readonly BuildTargetAdvancedCollection buildTargets;
        private readonly string scriptName;
        private readonly ESMAnalyzer esmAnalyzer;
        /*
        * TranspileScriptJob constructor.
        */
        public TranspileScriptJob(BuildTargetAdvancedCollection buildTargets, string scriptName)
        {
            this.buildTargets = buildTargets;
            this.scriptName = scriptName;
            this.esmAnalyzer = buildTargets.ESMAnalyzer;
        }

        public void Run()
        {
            string[] scripts = this.buildTargets.GetScriptsToCompile(this.scriptName);
            //BuildSourceFilesCollection partitionedScripts = this.buildTargets.GetSourceFiles(scripts);
            TES5GlobalVariableCollection globalVariables = this.esmAnalyzer.GlobalVariables;
            foreach (var buildTarget in this.buildTargets)
            {
                Dictionary<string, TES5GlobalScope> scriptsScopes = new Dictionary<string, TES5GlobalScope>();
                foreach (var buildScript in scripts)
                {
                    string scriptName = Path.GetFileNameWithoutExtension(buildScript);
                    string sourcePath = buildTarget.GetSourceFromPath(scriptName);
                    //string outputPath = buildTarget.GetTranspileToPath(scriptName);
                    TES5GlobalScope globalScope = buildTarget.BuildScope(sourcePath, globalVariables);
                    scriptsScopes.Add(scriptName, globalScope);
                }

                TES5MultipleScriptsScope multipleScriptsScope = new TES5MultipleScriptsScope(scriptsScopes.Values, globalVariables);
                foreach (var buildScript in scripts)
                {
                    string scriptName = Path.GetFileNameWithoutExtension(buildScript);
                    string sourcePath = buildTarget.GetSourceFromPath(scriptName);
                    string outputPath = buildTarget.GetTranspileToPath(scriptName);
                    TES5GlobalScope globalScope = buildTarget.BuildScope(sourcePath, globalVariables);
                    buildTarget.Transpile(sourcePath, outputPath, globalScope, multipleScriptsScope);
                }
            }
        }
    }
}