using Dissect.Parser.Exceptions;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.Extensions.StreamExtensions;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using System;
using System.Collections.Generic;
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
        private BuildLogServices buildLogServices;
        private ESMAnalyzer esmAnalyzer;
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
            this.esmAnalyzer = new ESMAnalyzer(DataDirectory.TES4GameFileName);
        }

        public void runTask(FileStream errorLog, ProgressWriter progressWriter)
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
                        string scriptName = Path.GetFileNameWithoutExtension(buildScript);
                        string sourcePath = buildTarget.getSourceFromPath(scriptName);
                        scriptsScopes.Add(scriptName, buildTarget.buildScope(sourcePath, globalVariables));
                    }
                }

                //Add the static global scopes which are added by complimenting scripts..
                List<TES5GlobalScope> staticGlobalScopes = TES5StaticGlobalScopesFactory.createGlobalScopes();
                //WTM:  Change:  In the PHP, scriptsScopes is used as a dictionary above but as a list below.  I have added the "GlobalScope"+n key to ameliorate this.
                //But I doubt these scripts are ever accessed.
                int globalScopeIndex = 0;
                foreach (var staticGlobalScope in staticGlobalScopes)
                {
                    scriptsScopes.Add("GlobalScope" + globalScopeIndex.ToString(), staticGlobalScope);
                    globalScopeIndex++;
                }

                TES5MultipleScriptsScope multipleScriptsScope = new TES5MultipleScriptsScope(scriptsScopes.Select(s => s.Value), globalVariables);
                //Dictionary<string, TES5Target> convertedScripts = new Dictionary<string, TES5Target>();
                foreach (var kvp in buildChunk)
                {
                    var buildTargetName = kvp.Key;
                    var buildScripts = kvp.Value;
                    foreach (var buildScript in buildScripts)
                    {
                        BuildTarget buildTarget = this.getBuildTarget(buildTargetName);
                        string scriptName = Path.GetFileNameWithoutExtension(buildScript);
                        TES5GlobalScope globalScope = scriptsScopes[scriptName];
                        string sourcePath = buildTarget.getSourceFromPath(scriptName);
                        string outputPath = buildTarget.getTranspileToPath(scriptName);
                        TES5Target convertedScript;
                        try
                        {
                            convertedScript = buildTarget.transpile(sourcePath, outputPath, globalScope, multipleScriptsScope);
                        }
                        catch (EOFOnlyException) { continue; }//Ignore files that are only whitespace or comments.
#if !DEBUG || LOG_EXCEPTIONS
                        catch (ConversionException ex) when (ex.Expected)
                        {
                            errorLog.WriteUTF8(scriptName + " (" + sourcePath + ")\r\n" + ex.GetType().FullName + "\r\n" + ex.Message + "\r\n\r\n");
                            continue;
                        }
#endif
                        this.buildTracker.registerBuiltScript(buildTarget, convertedScript);
                        //convertedScripts.Add(buildScript, convertedScript);
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

        private BuildTarget getBuildTarget(string targetName)
        {
            if (this.buildTargets.getByNameOrNull(targetName) == null)
            {
                this.buildTargets.add(BuildTargetFactory.get(targetName, this.build, buildLogServices));
            }

            BuildTarget result = this.buildTargets.getByName(targetName);
            return result;
        }
    }
}