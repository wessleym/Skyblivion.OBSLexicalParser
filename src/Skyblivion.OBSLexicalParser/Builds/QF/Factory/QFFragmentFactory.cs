using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory
{
    class QFFragmentFactory
    {
        private readonly MappedTargetsLogService mappedTargetsLogService;
        private readonly ObjectiveHandlingFactory objectiveHandlingFactory;
        private readonly ESMAnalyzer esmAnalyzer;
        public QFFragmentFactory(MappedTargetsLogService mappedTargetsLogService, ObjectiveHandlingFactory objectiveHandlingFactory, ESMAnalyzer esmAnalyzer)
        {
            this.mappedTargetsLogService = mappedTargetsLogService;
            this.objectiveHandlingFactory = objectiveHandlingFactory;
            this.esmAnalyzer = esmAnalyzer;
        }

        /*
        * Joins N QF subfragments into one QF fragment that can be properly binded into Skyrim VM
        * @throws ConversionException
        */
        public TES5Target JoinQFFragments(BuildTarget target, string resultingFragmentName, List<QuestStageScript> subfragmentsTrees)
        {
            StageMap stageMap = BuildStageMap(target, resultingFragmentName);
            /*
             * We need script fragment for objective handling for each stage, so when parsing the script fragments,
             * we"ll be marking them there, and intersecting this with stage.
             * This will give us an array of stages which don"t have script fragment, but will need it anyways
             * for objective handling.
             */
            TES5ScriptHeader resultingScriptHeader = new TES5ScriptHeader(resultingFragmentName, TES5BasicType.T_QUEST, "", true, esmAnalyzer);
            TES5BlockList resultingBlockList = new TES5BlockList();
            TES5GlobalScope resultingGlobalScope = new TES5GlobalScope(resultingScriptHeader);
            /*
             * Add ReferenceAlias"es
             * At some point, we might port the conversion so it doesn"t use the directly injected property,
             * but instead has a map to aliases and we"ll map accordingly and have references point to aliases instead
             */
            string sourcePath = target.GetSourceFromPath(resultingFragmentName);
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath);
            string aliasesFile = Path.Combine(Path.GetDirectoryName(sourcePath), scriptName + ".aliases");
            string[] aliasesLines = File.ReadAllLines(aliasesFile);
            Dictionary<string, bool> aliasesDeclared = new Dictionary<string, bool>();
            foreach (var alias in aliasesLines)
            {
                string trimmedAlias = alias.Trim();
                if (trimmedAlias == "") { continue; }
                try
                {
                    aliasesDeclared.Add(trimmedAlias, true);
                }
                catch (ArgumentException) { continue; }
                resultingGlobalScope.AddProperty(new TES5Property(trimmedAlias, TES5BasicType.T_REFERENCEALIAS, trimmedAlias));
            }

            Dictionary<int, bool> implementedStages = new Dictionary<int, bool>();
            Dictionary<string, bool> propertiesNamesDeclared = new Dictionary<string, bool>();
            foreach (var subfragment in subfragmentsTrees)
            {
                TES5Target subfragmentsTree = subfragment.Script;
                TES5Script subfragmentScript = subfragmentsTree.Script;
                TES5GlobalScope subfragmentGlobalScope = subfragmentScript.GlobalScope;
                foreach (TES5Property subfragmentProperty in subfragmentGlobalScope.Properties)
                {
                    /*
                     * Move over the properties to the new global scope
                     */
                    string propertyName;
                    if (propertiesNamesDeclared.ContainsKey(subfragmentProperty.Name))
                    {
                        propertyName = GeneratePropertyName(subfragmentScript.ScriptHeader, subfragmentProperty);
                        subfragmentProperty.Rename(propertyName);
                    }
                    else
                    {
                        propertyName = subfragmentProperty.Name;
                    }

                    bool newProperty = false;//WTM:  Note:  At this time, the only non-new property is PlayerRef.
                    try
                    {
                        propertiesNamesDeclared.Add(propertyName, true);
                        newProperty = true;
                    }
                    catch (ArgumentException) when (subfragmentProperty.IsPlayerRef) { }
                    if (newProperty)
                    {
                        resultingGlobalScope.AddProperty(subfragmentProperty);
                        //WTM:  Note:  See QF_FGD03Viranus_0102d154.  Since ViranusDontonREF is present in multiple of the original fragments,
                        //ViranusDontonREF gets renamed by the above.  So multiple ViranusDontonREF variables are output.
                        //Below I tried not renaming, assuming instead that variables with matching names and types within a set of fragments were intended to be the same variable.
                        //It had OK results, but I'm leaving it commented for now.
                        /*string propertyNameWithSuffix = subfragmentProperty.PropertyNameWithSuffix;
                        TES5Property existingProperty = resultingGlobalScope.Properties.Where(p => p.PropertyNameWithSuffix == propertyNameWithSuffix).FirstOrDefault();
                        if (existingProperty != null && TES5InheritanceGraphAnalyzer.isExtending(subfragmentProperty.PropertyType, existingProperty.PropertyType))
                        {
                            existingProperty.PropertyType = subfragmentProperty.PropertyType;
                        }
                        else
                        {
                            bool add = true;
                            if (existingProperty != null)
                            {
                                if (TES5InheritanceGraphAnalyzer.isExtending(existingProperty.PropertyType, subfragmentProperty.PropertyType))
                                {
                                    add = false;
                                }
                                else
                                {
                                    string generatedPropertyName = generatePropertyName(subfragmentScript.ScriptHeader, subfragmentProperty, i);
                                    subfragmentProperty.Rename(generatedPropertyName);
                                }
                            }
                            if (add)
                            {
                                resultingGlobalScope.Add(subfragmentProperty);
                            }
                        }*/
                    }
                }

                List<ITES5CodeBlock> subfragmentBlocks = subfragmentScript.BlockList.Blocks;
                if (subfragmentBlocks.Count != 1)
                {
                    throw new ConversionException("Wrong QF fragment, actual function count: " + subfragmentBlocks.Count + "..");
                }

                ITES5CodeBlock subfragmentBlock = subfragmentBlocks[0];
                if (subfragmentBlock.FunctionScope.BlockName != "Fragment_0")
                {
                    throw new ConversionException("Wrong QF fragment funcname, actual function name: " + subfragmentBlock.FunctionScope.BlockName + "..");
                }

                string newFragmentFunctionName = "Fragment_" + subfragment.Stage.ToString();
                if (subfragment.LogIndex != 0)
                {
                    newFragmentFunctionName += "_" + subfragment.LogIndex;
                }

                subfragmentBlock.FunctionScope.Rename(newFragmentFunctionName);
                var objectiveCodeChunks = objectiveHandlingFactory.GenerateObjectiveHandling(subfragmentBlock, resultingGlobalScope, stageMap.GetStageTargetsMap(subfragment.Stage));
                foreach (var newCodeChunk in objectiveCodeChunks)
                {
                    subfragmentBlock.AddChunk(newCodeChunk);
                }

                resultingBlockList.Add(subfragmentBlock);
                implementedStages[subfragment.Stage] = true;
            }

            /*
             * Diff to find stages which we still need to mark
             */
            int[] nonDoneStages = stageMap.StageIDs.Where(stageID => !implementedStages.ContainsKey(stageID)).ToArray();
            foreach (int nonDoneStage in nonDoneStages)
            {
                TES5FunctionCodeBlock fragment = objectiveHandlingFactory.CreateEnclosedFragment(resultingGlobalScope, nonDoneStage, stageMap.GetStageTargetsMap(nonDoneStage));
                resultingBlockList.Add(fragment);
            }

            this.mappedTargetsLogService.WriteScriptName(resultingFragmentName);
            foreach (var kvp in stageMap.MappedTargetsIndex)
            {
                var originalTargetIndex = kvp.Key;
                var mappedTargetIndexes = kvp.Value;
                this.mappedTargetsLogService.WriteLine(originalTargetIndex, mappedTargetIndexes);
            }

            TES5Script resultingTree = new TES5Script(resultingGlobalScope, resultingBlockList);
            string outputPath = target.GetTranspileToPath(resultingFragmentName);
            return new TES5Target(resultingTree, outputPath);
        }

        private static string GeneratePropertyName(TES5ScriptHeader header, TES5Property property)
        {
            if (property.AllowNameTransformation)
            {
                return "col_" + property.OriginalName + "_" + PHPFunction.MD5(header.EscapedScriptName).Substring(0, 4);
                //WTM:  Note:  Instead of using an MD5 hash, I tried the below (where index was the property index within the script's TES5GlobalScope).
                //"col_" + property.OriginalName + "_" + header.EscapedScriptName + "_" + index
                //It worked, but I don't think the names matched up well with what GECK generates, so I've commented it for now.
            }
            return property.OriginalName;
        }

        public static Dictionary<int, List<int>> BuildStageMapDictionary(BuildTarget target, string resultingFragmentName)
        {
            string sourcePath = target.GetSourceFromPath(resultingFragmentName);
            //ToLower() is needed for Linux's case-sensitive file system since these files seem to all be lowercase.
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath).ToLower();
            string stageMapFile = Path.Combine(Path.GetDirectoryName(sourcePath), scriptName + ".map");
            string[] stageMapFileLines = File.ReadAllLines(stageMapFile);
            Dictionary<int, List<int>> stageMap = new Dictionary<int, List<int>>();
            foreach (var stageMapLine in stageMapFileLines)
            {
                string[] numberAndItemsSplit = stageMapLine.Split('-');
                int stageId = int.Parse(numberAndItemsSplit[0].Trim(), CultureInfo.InvariantCulture);
                /*
                 * Clear the rows
                 */
                string[] items = numberAndItemsSplit[1].Split(' ');
                List<int> stageRows = new List<int>();
                foreach (string item in items)
                {
                    string itemTrimmed = item.Trim();
                    if (itemTrimmed != "")
                    {
                        stageRows.Add(int.Parse(itemTrimmed, CultureInfo.InvariantCulture));
                    }
                }

                stageMap.Add(stageId, stageRows);
            }

            return stageMap;
        }

        private static StageMap BuildStageMap(BuildTarget target, string resultingFragmentName)
        {
            Dictionary<int, List<int>> stageMap = BuildStageMapDictionary(target, resultingFragmentName);
            return new StageMap(stageMap);
        }
    }
}