using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
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
        private readonly List<Dictionary<BuildTargetAdvanced, List<string>>> buildPlan;
        private readonly ESMAnalyzer esmAnalyzer;
        /*
        * No injection is done here because of multithreaded enviroment which messes it up.
        * Maybe at some point we will have a proper DI into the jobs.
        * TranspileChunkJob constructor.
        */
        public TranspileChunkJob(BuildTracker buildTracker, List<Dictionary<BuildTargetAdvanced, List<string>>> buildPlan, ESMAnalyzer esmAnalyzer)
        {
            this.buildPlan = buildPlan;
            this.buildTracker = buildTracker;
            this.esmAnalyzer = esmAnalyzer;
        }

        public void RunTask(StreamWriter errorLog, ProgressWriter progressWriter)
        {
            TES5GlobalVariableCollection globalVariables = this.esmAnalyzer.GlobalVariables;
            foreach (var buildChunk in this.buildPlan)
            {
                Dictionary<string, TES5GlobalScope> scriptsScopes = new Dictionary<string, TES5GlobalScope>();
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
                        TES5GlobalScope globalScope = buildTarget.BuildScope(sourcePath, globalVariables);
                        scriptsScopes.Add(scriptName, globalScope);
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
                        catch (EOFOnlyException) { }//Ignore files that are empty.
#if !DEBUG || LOGEXCEPTIONS
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