using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class TranspileChunkJob
    {
        private BuildTracker buildTracker;
        private BuildTargetCollection buildTargets;
        private List<Dictionary<string, List<string>>> buildPlan;
        private Build build;
        private TES5StaticGlobalScopesFactory staticGlobalScopesFactory;
        private ESMAnalyzer esmAnalyzer;
        /*
        * No injection is done here because of multithreaded enviroment which fucks it up.
        * Maybe at some point we will have a proper DI into the jobs.
        * TranspileChunkJob constructor.
        */
        public TranspileChunkJob(BuildTracker buildTracker, string buildPath, List<Dictionary<string, List<string>>> buildPlan)
        {
            this.buildPlan = buildPlan;
            this.buildTracker = buildTracker;
            this.build = new Build(buildPath);
            this.staticGlobalScopesFactory = new TES5StaticGlobalScopesFactory();
            this.buildTargets = new BuildTargetCollection();
            TypeMapper typeMapper = new TypeMapper();
            this.esmAnalyzer = new ESMAnalyzer(typeMapper, "Oblivion.esm");
        }

        public void runTask()
        {
            foreach (var buildChunk in this.buildPlan)
            {
                Dictionary<string, TES5GlobalScope> scriptsScopes = new Dictionary<string, TES5GlobalScope>();
                TES5GlobalVariables globalVariables = this.esmAnalyzer.getGlobalVariables();
                /*
                 * First, build the scripts global scopes
                 */
                foreach (var kvp in buildChunk)
                {
                    var buildTargetName = kvp.Key;
                    var buildScripts = kvp.Value;
                    BuildTarget buildTarget = this.getBuildTarget(buildTargetName);
                    foreach (var buildScript in buildScripts)
                    {
                        string scriptName = Path.GetFileName(buildScript);
                        string sourcePath = buildTarget.getSourceFromPath(scriptName);
                        scriptsScopes[scriptName] = buildTarget.buildScope(sourcePath, globalVariables);
                    }
                }

                //Add the static global scopes which are added by complimenting scripts..
                List<TES5GlobalScope> staticGlobalScopes = this.staticGlobalScopesFactory.createGlobalScopes();
                //WTM:  Change:  In the PHP, scriptsScopes is used as a dictionary above but as a list below.  I have added the "GlobalScope"+n key to ameliorate this.
                //But I don't think the scripts added below are ever used.
                int globalScopeIndex = 0;
                foreach (var staticGlobalScope in staticGlobalScopes)
                {
                    scriptsScopes.Add("GlobalScope" + globalScopeIndex.ToString(), staticGlobalScope);
                    globalScopeIndex++;
                }

                TES5MultipleScriptsScope multipleScriptsScope = new TES5MultipleScriptsScope(scriptsScopes.Select(s => s.Value), globalVariables);
                Dictionary<string, TES5Target> convertedScripts = new Dictionary<string, TES5Target>();
                foreach (var kvp in buildChunk)
                {
                    var buildTargetName = kvp.Key;
                    var buildScripts = kvp.Value;
                    foreach (var buildScript in buildScripts)
                    {
                        BuildTarget buildTarget = this.getBuildTarget(buildTargetName);
                        string scriptName = Path.GetFileName(buildScript);
                        var globalScope = scriptsScopes[scriptName];
                        string sourcePath = buildTarget.getSourceFromPath(scriptName);
                        string outputPath = buildTarget.getTranspileToPath(scriptName);
                        try
                        {
                            TES5Target convertedScript = buildTarget.transpile(sourcePath, outputPath, globalScope, multipleScriptsScope);
                            convertedScripts[buildScript] = convertedScript;
                            this.buildTracker.registerBuiltScript(buildTarget, convertedScript);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception:  " + e.GetType().FullName+":  "+e.Message);
                        }
                    }

                    foreach (var kvp2 in convertedScripts)
                    {
                        var originalScriptName = kvp2.Key;
                        var convertedScript = kvp2.Value;
                        Console.WriteLine("Script Complete:  " + originalScriptName);
                    }
                }
            }
        }

        private BuildTarget getBuildTarget(string targetName)
        {
            if (this.buildTargets.getByName(targetName) == null)
            {
                this.buildTargets.add(BuildTargetFactory.get(targetName, this.build));
            }

            BuildTarget result = this.buildTargets.getByName(targetName);
            if (result == null)
            {
                throw new InvalidOperationException("Unknown build " + targetName);
            }

            return result;
        }
    }
}