using Dissect.Extensions.IDictionaryExtensions;
using Dissect.Parser.Exceptions;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Extensions.StreamExtensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Graph;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class BuildInteroperableCompilationGraphs : LPCommand
    {
        public BuildInteroperableCompilationGraphs()
            : base("skyblivion:parser:buildGraphs", "Build Interoperable Compilation Graphs", "Build graphs of scripts which are interconnected to be transpiled together")
        {
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTarget.DEFAULT_TARGETS));
        }

        public void execute(LPCommandInput input)
        {
            string targets = input.GetArgumentValue("targets");
            execute(targets);
        }

        public override void execute()
        {
            execute(BuildTarget.DEFAULT_TARGETS);
        }

        public void execute(string targets)
        {
            Build build = new Build(Build.DEFAULT_BUILD_PATH); //This argument might well not be important in this case
            using (BuildLogServices buildLogServices = new BuildLogServices(build))
            {
                BuildTargetCollection buildTargets = BuildTargetFactory.getCollection(targets, build, buildLogServices);
                if (!buildTargets.canBuild())
                {
                    Console.WriteLine("Targets current build directory not clean.  Archive them manually, or run clean.sh.");
                    return;
                }
                Dictionary<string, List<string>> dependencyGraph = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> usageGraph = new Dictionary<string, List<string>>();
                BuildSourceFilesCollection sourceFiles = buildTargets.getSourceFiles();
                ProgressWriter progressWriter = new ProgressWriter("Building Interoperable Compilation Graph", buildTargets.getTotalSourceFiles());
                TES5TypeInferencer inferencer = new TES5TypeInferencer(new ESMAnalyzer(new TypeMapper()), BuildTarget.StandaloneSourcePath);
                using (FileStream errorLog = new FileStream(TES5ScriptDependencyGraph.ErrorLogPath, FileMode.Create))
                {
                    using (FileStream debugLog = new FileStream(TES5ScriptDependencyGraph.DebugLogPath, FileMode.Create))
                    {
                        foreach (var kvp in sourceFiles)
                        {
                            var buildTargetName = kvp.Key;
                            var sourceBuildFiles = kvp.Value;
                            BuildTarget buildTarget = buildTargets.getByName(buildTargetName);
                            foreach (var sourceFile in sourceBuildFiles)
                            {
                                string scriptName = sourceFile.Substring(0, sourceFile.Length - 4);
                                ITES4CodeFilterable AST;
                                try
                                {
                                    AST = buildTarget.getAST(buildTarget.getSourceFromPath(scriptName));
                                }
                                catch (EOFOnlyException) { continue; }//Ignore files that are only whitespace or comments.
                                /*catch (UnexpectedTokenException ex)
                                {//Exceptions no longer occur, so this code should not be invoked.
                                    errorLog.WriteUTF8(sourceFile + ":  " + ex.Message + "\r\n");
                                    continue;
                                }*/
                                List<TES4ObjectProperty> propertiesAccesses = new List<TES4ObjectProperty>();
                                AST.filter((data) =>
                                {
                                    TES4ObjectProperty property = data as TES4ObjectProperty;
                                    if (property == null) { return false; }
                                    propertiesAccesses.Add(property);
                                    return true;
                                });
                                Dictionary<string, TES5Property> preparedProperties = new Dictionary<string, TES5Property>();
                                Dictionary<string, ITES5Type> preparedPropertiesTypes = new Dictionary<string, ITES5Type>();
                                foreach (var property in propertiesAccesses)
                                {
                                    Match match = Regex.Match(property.StringValue, @"([0-9a-zA-Z]+)\.([0-9a-zA-Z]+)", RegexOptions.IgnoreCase);
                                    string propertyName = match.Groups[1].Value;
                                    string propertyKeyName = propertyName.ToLower();
                                    bool containedKey;
                                    TES5Property preparedProperty = preparedProperties.GetOrAdd(propertyKeyName, () => new TES5Property(propertyName, TES5BasicType.T_FORM, propertyName), out containedKey);
                                    ITES5Type inferencingType = inferencer.resolveInferenceTypeByReferenceEdid(preparedProperty);
                                    if (!containedKey)
                                    {
                                        preparedPropertiesTypes.Add(propertyKeyName, inferencingType);
                                    }
                                    else
                                    {
                                        if (!inferencingType.Equals(preparedPropertiesTypes[propertyKeyName]))
                                        {
                                            throw new ConversionException("Cannot settle up the properties types - conflict.");
                                        }
                                    }
                                }

                                debugLog.WriteUTF8(scriptName + " - " + preparedProperties.Count + " prepared\r\n");
                                foreach (var kvp2 in preparedProperties)
                                {
                                    var preparedPropertyKey = kvp2.Key;
                                    var preparedProperty = kvp2.Value;
                                    string propertyTypeName = preparedPropertiesTypes[preparedPropertyKey].getOriginalName();
                                    //Only keys are lowercased.
                                    string lowerPropertyType = propertyTypeName.ToLower();
                                    string lowerScriptType = scriptName.ToLower();
                                    dependencyGraph.AddNewListIfNotContainsKeyAndAddValueToList(lowerPropertyType, lowerScriptType);
                                    usageGraph.AddNewListIfNotContainsKeyAndAddValueToList(lowerScriptType, lowerPropertyType);
                                    debugLog.WriteUTF8("Registering a dependency from " + scriptName + " to " + propertyTypeName + "\r\n");
                                }

                                progressWriter.IncrementAndWrite();
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
}