using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory
{
    class QFFragmentFactory
    {
        private MappedTargetsLogService mappedTargetsLogService;
        private ObjectiveHandlingFactory objectiveHandlingFactory;
        public QFFragmentFactory(MappedTargetsLogService mappedTargetsLogService, ObjectiveHandlingFactory objectiveHandlingFactory)
        {
            this.mappedTargetsLogService = mappedTargetsLogService;
            this.objectiveHandlingFactory = objectiveHandlingFactory;
        }

        /*
        * Joins N QF subfragments into one QF fragment that can be properly binded into Skyrim VM
        * @throws ConversionException
        */
        public TES5Target joinQFFragments(BuildTarget target, string resultingFragmentName, List<QuestStageScript> subfragmentsTrees)
        {
            StageMap stageMap = BuildStageMap(target, resultingFragmentName);
            /*
             * We need script fragment for objective handling for each stage, so when parsing the script fragments,
             * we"ll be marking them there, and intersecting this with stage.
             * This will give us an array of stages which don"t have script fragment, but will need it anyways
             * for objective handling.
             */
            TES5ScriptHeader resultingScriptHeader = new TES5ScriptHeader(resultingFragmentName, TES5BasicType.T_QUEST, "", true);
            TES5BlockList resultingBlockList = new TES5BlockList();
            TES5GlobalScope resultingGlobalScope = new TES5GlobalScope(resultingScriptHeader);
            /*
             * Add ReferenceAlias"es
             * At some point, we might port the conversion so it doesn"t use the directly injected property,
             * but instead has a map to aliases and we"ll map accordingly and have references point to aliases instead
             */
            string sourcePath = target.getSourceFromPath(resultingFragmentName);
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
                resultingGlobalScope.add(new TES5Property(trimmedAlias, TES5BasicType.T_REFERENCEALIAS, trimmedAlias));
            }

            Dictionary<int, bool> implementedStages = new Dictionary<int, bool>();
            Dictionary<string, bool> propertiesNamesDeclared = new Dictionary<string, bool>();
            foreach (var subfragment in subfragmentsTrees)
            {
                TES5Target subfragmentsTree = subfragment.getScript();
                TES5Script subfragmentScript = subfragmentsTree.Script;
                TES5GlobalScope subfragmentGlobalScope = subfragmentScript.GlobalScope;
                for (int i = 0; i < subfragmentGlobalScope.Properties.Count; i++)
                {
                    TES5Property subfragmentProperty = subfragmentGlobalScope.Properties[i];
                    /*
                     * Move over the properties to the new global scope
                     */
                    string propertyName;
                    if (propertiesNamesDeclared.ContainsKey(subfragmentProperty.getPropertyName()))
                    {
                        propertyName = generatePropertyName(subfragmentScript.ScriptHeader, subfragmentProperty, i);
                        subfragmentProperty.renameTo(propertyName);
                    }
                    else
                    {
                        propertyName = subfragmentProperty.getPropertyName();
                    }

                    propertiesNamesDeclared.Add(propertyName, true);
                    resultingGlobalScope.add(subfragmentProperty);
                }

                List<ITES5CodeBlock> subfragmentBlocks = subfragmentScript.BlockList.getBlocks();
                if (subfragmentBlocks.Count != 1)
                {
                    throw new ConversionException("Wrong QF fragment, actual function count: " + subfragmentBlocks.Count + "..");
                }

                ITES5CodeBlock subfragmentBlock = subfragmentBlocks[0];
                if (subfragmentBlock.getFunctionScope().getBlockName() != "Fragment_0")
                {
                    throw new ConversionException("Wrong QF fragment funcname, actual function name: " + subfragmentBlock.getFunctionScope().getBlockName() + "..");
                }

                string newFragmentFunctionName = "Fragment_" + subfragment.getStage().ToString();
                if (subfragment.getLogIndex() != 0)
                {
                    newFragmentFunctionName += "_" + subfragment.getLogIndex();
                }

                subfragmentBlock.getFunctionScope().renameTo(newFragmentFunctionName);
                var objectiveCodeChunks = this.objectiveHandlingFactory.generateObjectiveHandling(subfragmentBlock, resultingGlobalScope, stageMap.getStageTargetsMap(subfragment.getStage()));
                foreach (var newCodeChunk in objectiveCodeChunks)
                {
                    subfragmentBlock.addChunk(newCodeChunk);
                }

                resultingBlockList.add(subfragmentBlock);
                implementedStages[subfragment.getStage()] = true;
            }

            /*
             * Diff to find stages which we still need to mark
             */
            int[] nonDoneStages = stageMap.getStageIds().Where(stageID => !implementedStages.ContainsKey(stageID)).ToArray();
            foreach (int nonDoneStage in nonDoneStages)
            {
                TES5FunctionCodeBlock fragment = this.objectiveHandlingFactory.createEnclosedFragment(resultingGlobalScope, nonDoneStage, stageMap.getStageTargetsMap(nonDoneStage));
                resultingBlockList.add(fragment);
            }

            this.mappedTargetsLogService.WriteScriptName(resultingFragmentName);
            foreach (var kvp in stageMap.getMappedTargetsIndex())
            {
                var originalTargetIndex = kvp.Key;
                var mappedTargetIndexes = kvp.Value;
                this.mappedTargetsLogService.WriteLine(originalTargetIndex, mappedTargetIndexes);
            }

            TES5Script resultingTree = new TES5Script(resultingGlobalScope, resultingBlockList);
            string outputPath = target.getTranspileToPath(resultingFragmentName);
            return new TES5Target(resultingTree, outputPath);
        }

        private static string generatePropertyName(TES5ScriptHeader header, TES5Property property, int index)
        {
            return "col_" + property.GetPropertyNameWithoutSuffix() + "_" +
#if PHP_COMPAT
                PHPFunction.MD5(header.getScriptName()).Substring(0, 4)
#else
                header.EscapedScriptName + "_" + index;
#endif
                ;
        }

        public static Dictionary<int, List<int>> BuildStageMapDictionary(BuildTarget target, string resultingFragmentName)
        {
            string sourcePath = target.getSourceFromPath(resultingFragmentName);
            //ToLower() is needed for Linux's case-sensitive file system since these files seem to all be lowercase.
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath).ToLower();
            string stageMapFile = Path.Combine(Path.GetDirectoryName(sourcePath), scriptName + ".map");
            string[] stageMapFileLines = File.ReadAllLines(stageMapFile);
            Dictionary<int, List<int>> stageMap = new Dictionary<int, List<int>>();
            foreach (var stageMapLine in stageMapFileLines)
            {
                string[] numberAndItemsSplit = stageMapLine.Split('-');
                int stageId = int.Parse(numberAndItemsSplit[0].Trim());
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
                        stageRows.Add(int.Parse(itemTrimmed));
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