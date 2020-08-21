using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands.Dispatch
{
    class TranspileChunkJob
    {
        private readonly BuildTracker buildTracker;
        private readonly BuildTargetAdvancedCollection buildTargets;
        private readonly List<Dictionary<BuildTargetAdvanced, List<string>>> buildPlan;
        private readonly Build build;
        private readonly BuildLogServiceCollection buildLogServices;
        private readonly ESMAnalyzer esmAnalyzer;
        private readonly TES5TypeInferencer typeInferencer;
        /*
        * No injection is done here because of multithreaded enviroment which messes it up.
        * Maybe at some point we will have a proper DI into the jobs.
        * TranspileChunkJob constructor.
        */
        public TranspileChunkJob(Build build, BuildTracker buildTracker, BuildLogServiceCollection buildLogServices, List<Dictionary<BuildTargetAdvanced, List<string>>> buildPlan, ESMAnalyzer esmAnalyzer, TES5TypeInferencer typeInferencer)
        {
            this.buildPlan = buildPlan;
            this.buildTracker = buildTracker;
            this.build = build;
            this.buildLogServices = buildLogServices;
            this.esmAnalyzer = esmAnalyzer;
            this.typeInferencer = typeInferencer;
            this.buildTargets = new BuildTargetAdvancedCollection();
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
                    BuildTargetAdvanced buildTarget = kvp.Key;
                    List<string> buildScripts = kvp.Value;
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
                    BuildTargetAdvanced buildTarget = kvp.Key;
                    List<string> buildScripts = kvp.Value;
                    foreach (string buildScript in buildScripts)
                    {
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
    }
}