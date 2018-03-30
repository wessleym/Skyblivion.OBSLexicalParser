using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Context;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class TranspileScriptJob
    {
        private BuildTargetCollection buildTargets;
        private string scriptName;
        private ESMAnalyzer esmAnalyzer;
        /*
        * TranspileScriptJob constructor.
        */
        public TranspileScriptJob(BuildTargetCollection buildTargets, string scriptName)
        {
            this.buildTargets = buildTargets;
            this.scriptName = scriptName;
            TypeMapper typeMapper = new TypeMapper();
            this.esmAnalyzer = new ESMAnalyzer(typeMapper, DataDirectory.TES4GameFileName);
        }

        public void run()
        {
            string[] scripts = this.buildTargets.getScriptsToCompile(this.scriptName);
            BuildSourceFilesCollection partitionedScripts = this.buildTargets.getSourceFiles(scripts);
            TES5GlobalVariables globalVariables = this.esmAnalyzer.getGlobalVariables();
            foreach (var buildTarget in this.buildTargets)
            {
                Dictionary<string, TES5GlobalScope> scriptsScopes = new Dictionary<string, TES5GlobalScope>();
                foreach (var buildScript in scripts)
                {
                    string scriptName = Path.GetFileNameWithoutExtension(buildScript);
                    string sourcePath = buildTarget.getSourceFromPath(scriptName);
                    //string outputPath = buildTarget.getTranspileToPath(scriptName);
                    scriptsScopes.Add(scriptName, buildTarget.buildScope(sourcePath, globalVariables));
                }

                TES5MultipleScriptsScope multipleScriptsScope = new TES5MultipleScriptsScope(scriptsScopes.Values, globalVariables);
                foreach (var buildScript in scripts)
                {
                    string scriptName = Path.GetFileNameWithoutExtension(buildScript);
                    string sourcePath = buildTarget.getSourceFromPath(scriptName);
                    string outputPath = buildTarget.getTranspileToPath(scriptName);
                    TES5GlobalScope globalScope = buildTarget.buildScope(sourcePath, globalVariables);
                    buildTarget.transpile(sourcePath, outputPath, globalScope, multipleScriptsScope);
                }
            }
        }
    }
}