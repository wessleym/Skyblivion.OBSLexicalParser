using Dissect.Extensions;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Graph;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.Commands
{
    class BuildInteroperableCompilationGraphs : LPCommand
    {
        public const string FriendlyNameConst = "Build Interoperable Compilation Graphs";
        public BuildInteroperableCompilationGraphs()
            : base("skyblivion:parser:buildGraphs", FriendlyNameConst, "Build graphs of scripts which are interconnected to be transpiled together")
        {
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTargetFactory.DefaultNames));
        }

        public void Execute(LPCommandInput input)
        {
            string targets = input.GetArgumentValue("targets");
            Execute(targets);
        }

        public override void Execute()
        {
            Execute(BuildTargetFactory.DefaultNames);
        }

        public void Execute(string targets)
        {
            if (!PreExecutionChecks(true, true, false, false)) { return; }
            Directory.CreateDirectory(DataDirectory.GetGraphDirectoryPath());
            Build build = new Build(Build.DEFAULT_BUILD_PATH); //This argument might well not be important in this case
            BuildTarget[] buildTargets = BuildTargetFactory.ParseCollection(targets, build);
            BuildTargetSimple[] buildTargetsSimple = BuildTargetFactory.GetCollection(buildTargets);
            ProgressWriter? progressWriter = null;
            Dictionary<string, List<string>> dependencyGraph = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> usageGraph = new Dictionary<string, List<string>>();
            using (ESMAnalyzer esmAnalyzer = ESMAnalyzer.Load())
            {
                progressWriter = new ProgressWriter("Building Interoperable Compilation Graph", buildTargets.GetTotalSourceFiles());
                BuildSourceFilesCollection<BuildTargetSimple> sourceFiles = buildTargetsSimple.GetSourceFiles();
                using (StreamWriter errorLog = new StreamWriter(TES5ScriptDependencyGraph.ErrorLogPath, false))
                {
                    using (StreamWriter debugLog = new StreamWriter(TES5ScriptDependencyGraph.DebugLogPath, false))
                    {
                        foreach (var kvp in sourceFiles)
                        {
                            BuildTargetSimple buildTarget = kvp.Key;
                            var sourceBuildFiles = kvp.Value;
                            foreach (var sourceFile in sourceBuildFiles)
                            {
                                string scriptName = sourceFile.Substring(0, sourceFile.Length - 4);//remove extension
                                ITES4CodeFilterable ast;
                                try
                                {
                                    ast = buildTarget.GetAST(buildTarget.GetSourceFromPath(scriptName));
                                }
                                catch (EOFOnlyException) { continue; }//Ignore files that are only whitespace or comments.
                                /*catch (UnexpectedTokenException ex)
                                {//UnexpectedTokenExceptions no longer occur, so this code should not be invoked.
                                    errorLog.WriteLine(sourceFile + ":  " + ex.Message + Environment.NewLine);
                                    continue;
                                }*/
                                List<TES4ObjectProperty> propertiesAccesses = new List<TES4ObjectProperty>();
                                ast.Filter((data) =>
                                {
                                    TES4ObjectProperty? property = data as TES4ObjectProperty;
                                    if (property == null) { return false; }
                                    propertiesAccesses.Add(property);
                                    return true;
                                });
                                Dictionary<string, ITES5Type> preparedProperties = new Dictionary<string, ITES5Type>();
                                foreach (var property in propertiesAccesses)
                                {
                                    Match match = TES5ReferenceFactory.ReferenceAndPropertyNameRegex.Match(property.StringValue);
                                    string propertyName = match.Groups[1].Value;
                                    string propertyKeyName = propertyName.ToLower();
                                    preparedProperties.AddIfNotContainsKey(propertyKeyName, () => esmAnalyzer.GetScriptTypeByEDID(propertyName));
                                }

                                debugLog.WriteLine(scriptName + " - " + preparedProperties.Count + " prepared");
                                string lowerScriptType = scriptName.ToLower();
                                foreach (var kvp2 in preparedProperties)
                                {
                                    var preparedPropertyKey = kvp2.Key;
                                    string propertyTypeName = preparedProperties[preparedPropertyKey].OriginalName;
                                    //Only keys are lowercased.
                                    string lowerPropertyType = propertyTypeName.ToLower();
                                    dependencyGraph.AddNewListIfNotContainsKeyAndAddValueToList(lowerPropertyType, lowerScriptType);
                                    usageGraph.AddNewListIfNotContainsKeyAndAddValueToList(lowerScriptType, lowerPropertyType);
                                    debugLog.WriteLine("Registering a dependency from " + scriptName + " to " + propertyTypeName);
                                }

                                progressWriter.IncrementAndWrite();
                            }
                        }
                    }
                }
            }
            progressWriter.Write("Saving");
            TES5ScriptDependencyGraph graph = new TES5ScriptDependencyGraph(dependencyGraph, usageGraph);
            buildTargets.WriteGraph(graph);
            progressWriter.WriteLast();
        }
    }
}