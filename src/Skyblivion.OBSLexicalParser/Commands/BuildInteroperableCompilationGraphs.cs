using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Extensions.StreamExtensions;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess;
using Skyblivion.OBSLexicalParser.TES4.Context;
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
    class BuildInteroperableCompilationGraphs : LPCommand
    {
        protected BuildInteroperableCompilationGraphs()
        {
            Name = "skyblivion:parser:buildGraphs";
            Description = "Build graphs of scripts which are interconnected to be transpiled together";
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTarget.DEFAULT_TARGETS));
        }

        protected void execute(LPCommandInput input)
        {
            set_time_limit(10800);
            string targets = input.GetArgumentValue("targets");
            Build build = new Build(Build.DEFAULT_BUILD_PATH); //This argument might well not be important in this case
            BuildTargetCollection buildTargets = BuildTargetFactory.getCollection(targets, build);
            Dictionary<string, List<string>> dependencyGraph = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> usageGraph = new Dictionary<string, List<string>>();
            if (!buildTargets.canBuild())
            {
                Console.WriteLine("Targets current build dir not clean, archive it manually.");
                return;
            }
            BuildSourceFilesCollection sourceFiles = buildTargets.getSourceFiles();
            int sourceFilesCount = buildTargets.getTotalSourceFiles();
            TES5TypeInferencer inferencer = new TES5TypeInferencer(new ESMAnalyzer(new TypeMapper()), "./BuildTargets/Standalone/Source/");
            using (FileStream errorLog = new FileStream("graph_error_log", FileMode.Create))
            {
                using (FileStream log = new FileStream("graph_debug_log", FileMode.Create))
                {
                    foreach (var kvp in sourceFiles)
                    {
                        var buildTargetName = kvp.Key;
                        var sourceBuildFiles = kvp.Value;
                        int sourceFileIndex = 0;
                        foreach (var sourceFile in sourceBuildFiles)
                        {
                            try
                            {
                                BuildTarget buildTarget = buildTargets.getByName(buildTargetName);
                                string scriptName = sourceFile.Substring(0, sourceFile.Length - 4);
                                TES4Script AST = buildTarget.getAST(buildTarget.getSourceFromPath(scriptName));
                                List<TES4ObjectProperty> propertiesAccesses = new List<TES4ObjectProperty>();
                                AST.filter((data)=>
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
                                    Match match = Regex.Match((string)property.getData(), @"([0-9a-zA-Z]+)\.([0-9a-zA-Z]+)", RegexOptions.IgnoreCase);
                                    string propertyName = match.Groups[1].Value;
                                    string propertyKeyName = propertyName.ToLower();
                                    if (!preparedProperties.ContainsKey(propertyKeyName))
                                    {
                                        TES5Property preparedProperty = new TES5Property(propertyName, TES5BasicType.T_FORM, propertyName);
                                        preparedProperties[propertyKeyName] = preparedProperty;
                                        ITES5Type inferencingType = inferencer.resolveInferenceTypeByReferenceEdid(preparedProperty);
                                        preparedPropertiesTypes[propertyKeyName] = inferencingType;
                                    }
                                    else
                                    {
                                        TES5Property preparedProperty = preparedProperties[propertyKeyName];
                                        ITES5Type inferencingType = inferencer.resolveInferenceTypeByReferenceEdid(preparedProperty);
                                        if (inferencingType != preparedPropertiesTypes[propertyKeyName])
                                        {
                                            throw new ConversionException("Cannot settle up the properties types - conflict.");
                                        }
                                    }
                                }

                                log.WriteUTF8(scriptName + " - " + preparedProperties.Count + " prepared\r\n");
                                foreach (var kvp2 in preparedProperties)
                                {
                                    var preparedPropertyKey = kvp2.Key;
                                    var preparedProperty = kvp2.Value;
                                    //Only keys are lowercased.
                                    string lowerPropertyType = preparedPropertiesTypes[preparedPropertyKey].getOriginalName().ToLower();
                                    string lowerScriptType = scriptName.ToLower();
                                    if (!dependencyGraph.ContainsKey(lowerPropertyType))
                                    {
                                        dependencyGraph[lowerPropertyType] = new List<string>();
                                    }

                                    dependencyGraph[lowerPropertyType].Add(lowerScriptType);
                                    if (!usageGraph.ContainsKey(lowerScriptType))
                                    {
                                        usageGraph[lowerScriptType] = new List<string>();
                                    }

                                    usageGraph[lowerScriptType].Add(lowerPropertyType);
                                    log.WriteUTF8("Registering a dependency from " + scriptName + " to " + preparedPropertiesTypes[preparedPropertyKey].getOriginalName() + "\r\n");
                                }

                                sourceFileIndex++;
                                Console.WriteLine("Progress:  " + sourceFileIndex.ToString() + "/" + sourceFilesCount.ToString());
                            }
                            catch (Exception e)
                            {
                                errorLog.WriteUTF8(sourceFile + "\r\n" + e.Message + "\r\n");
                                continue;
                            }
                        }
                    }
                }
            }

            TES5ScriptDependencyGraph graph = new TES5ScriptDependencyGraph(dependencyGraph, usageGraph);
            File.WriteAllText("app/graph_" + buildTargets.getUniqueBuildFingerprint(), PHPFunction.Serialize(graph));
        }
    }
}