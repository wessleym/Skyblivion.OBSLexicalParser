using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using System;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class TranspileChunkJob
    {
        private readonly BuildTracker buildTracker;
        private readonly BuildTargetCollection buildTargets;
        private readonly List<Dictionary<string, List<string>>> buildPlan;
        private readonly Build build;
        private readonly BuildLogServices buildLogServices;
        private readonly ESMAnalyzer esmAnalyzer;
        /*
        * No injection is done here because of multithreaded enviroment which messes it up.
        * Maybe at some point we will have a proper DI into the jobs.
        * TranspileChunkJob constructor.
        */
        public TranspileChunkJob(Build build, BuildTracker buildTracker, BuildLogServices buildLogServices, List<Dictionary<string, List<string>>> buildPlan)
        {
            this.buildPlan = buildPlan;
            this.buildTracker = buildTracker;
            this.build = build;
            this.buildLogServices = buildLogServices;
            this.buildTargets = new BuildTargetCollection();
            this.esmAnalyzer = new ESMAnalyzer(false, DataDirectory.TES4GameFileName);
        }

        public void RunTask(StreamWriter errorLog, ProgressWriter progressWriter)
        {
            foreach (var buildChunk in this.buildPlan)
            {
                Dictionary<string, TES5GlobalScope> scriptsScopes = new Dictionary<string, TES5GlobalScope>();
                TES5GlobalVariables globalVariables = this.esmAnalyzer.GlobalVariables;
                /*
                 * First, build the scripts global scopes
                 */
                foreach (var kvp in buildChunk)
                {
                    var buildTargetName = kvp.Key;
                    var buildScripts = kvp.Value;
                    BuildTarget buildTarget = this.GetBuildTarget(buildTargetName);
                    foreach (var buildScript in buildScripts)
                    {
                        string scriptName = Path.GetFileNameWithoutExtension(buildScript);
                        string sourcePath = buildTarget.GetSourceFromPath(scriptName);
                        scriptsScopes.Add(scriptName, buildTarget.BuildScope(sourcePath, globalVariables));
                    }
                }

                //Add the static global scopes which are added by complimenting scripts..
                List<TES5GlobalScope> staticGlobalScopes = TES5StaticGlobalScopesFactory.CreateGlobalScopes();
                //WTM:  Change:  In the PHP, scriptsScopes is used as a dictionary above but as a list below.  I have added the "GlobalScope"+n key to ameliorate this.
                int globalScopeIndex = 0;
                foreach (var staticGlobalScope in staticGlobalScopes)
                {
                    scriptsScopes.Add("GlobalScope" + globalScopeIndex.ToString(), staticGlobalScope);
                    globalScopeIndex++;
                }

                TES5MultipleScriptsScope multipleScriptsScope = new TES5MultipleScriptsScope(scriptsScopes.Values, globalVariables);
                //Dictionary<string, TES5Target> convertedScripts = new Dictionary<string, TES5Target>();
                foreach (var kvp in buildChunk)
                {
                    var buildTargetName = kvp.Key;
                    var buildScripts = kvp.Value;
                    foreach (var buildScript in buildScripts)
                    {
                        BuildTarget buildTarget = this.GetBuildTarget(buildTargetName);
                        string scriptName = Path.GetFileNameWithoutExtension(buildScript);
                        TES5GlobalScope globalScope = scriptsScopes[scriptName];
                        string sourcePath = buildTarget.GetSourceFromPath(scriptName);
                        string outputPath = buildTarget.GetTranspileToPath(scriptName);
                        TES5Target? convertedScript = null;
                        try
                        {
                            convertedScript = buildTarget.Transpile(sourcePath, outputPath, globalScope, multipleScriptsScope);
                        }
                        catch (EOFOnlyException) { }//Ignore files that are only whitespace or comments.
#if !DEBUG || LOG_EXCEPTIONS
                        catch (ConversionException ex) when (ex.Expected)
                        {
                            errorLog.Write(scriptName + " (" + sourcePath + ")" + Environment.NewLine + ex.Message + Environment.NewLine + Environment.NewLine);
                        }
#endif
                        if (convertedScript != null)
                        {
                            this.buildTracker.RegisterBuiltScript(buildTarget, convertedScript);
                            //convertedScripts.Add(buildScript, convertedScript);
                        }
                        progressWriter.IncrementAndWrite();
                    }
                }

                /*foreach (var kvp2 in convertedScripts)
                {
                    var originalScriptName = kvp2.Key;
                    Console.WriteLine("Script Complete:  " + originalScriptName);
                }*/
            }
        }

        private BuildTarget GetBuildTarget(string targetName)
        {
            if (this.buildTargets.GetByNameOrNull(targetName) == null)
            {
                this.buildTargets.Add(BuildTargetFactory.Get(targetName, this.build, buildLogServices, false));
            }

            BuildTarget result = this.buildTargets.GetByName(targetName);
            return result;
        }
    }
}