using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
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
            StageMap stageMap = this.buildStageMap(target, resultingFragmentName);
            /*
             * We need script fragment for objective handling for each stage, so when parsing the script fragments,
             * we"ll be marking them there, and intersecting this with stage.
             * This will give us an array of stages which don"t have script fragment, but will need it anyways
             * for objective handling.
             */
            Dictionary<int, bool> implementedStages = new Dictionary<int, bool>();
            string outputPath = target.getTranspileToPath(resultingFragmentName);
            TES5ScriptHeader resultingScriptHeader = new TES5ScriptHeader(resultingFragmentName, resultingFragmentName, TES5BasicType.T_QUEST, "", true);
            TES5BlockList resultingBlockList = new TES5BlockList();
            TES5GlobalScope resultingGlobalScope = new TES5GlobalScope(resultingScriptHeader);
            /*
             * Add ReferenceAlias"es
             * At some point, we might port the conversion so it doesn"t use the directly injected property,
             * but instead has a map to aliases and we"ll map accordingly and have references point to aliases instead
             */
            string sourcePath = target.getSourceFromPath(resultingFragmentName);
            string scriptName = Path.GetFileName(sourcePath);
            string aliasesFile = Path.GetDirectoryName(sourcePath) + "/" + scriptName + ".aliases";
            string[] aliasesLines = File.ReadAllLines(aliasesFile);
            Dictionary<string, bool> aliasesDeclared = new Dictionary<string, bool>();
            foreach (var alias in aliasesLines)
            {
                string trimmedAlias = alias.Trim();
                if (trimmedAlias == "")
                {
                    continue;
                }

                if (aliasesDeclared.ContainsKey(trimmedAlias))
                {
                    continue;
                }

                resultingGlobalScope.add(new TES5Property(trimmedAlias, TES5BasicType.T_REFERENCEALIAS, trimmedAlias));
                aliasesDeclared.Add(trimmedAlias, true);
            }

            Dictionary<string, bool> propertiesNamesDeclared = new Dictionary<string, bool>();
            foreach (var subfragment in subfragmentsTrees)
            {
                TES5Target subfragmentsTree = subfragment.getScript();
                TES5Script subfragmentScript = subfragmentsTree.getScript();
                TES5GlobalScope subfragmentGlobalScope = subfragmentScript.getGlobalScope();
                foreach (var subfragmentProperty in subfragmentGlobalScope.getPropertiesList())
                {
                    /*
                     * Move over the properties to the new global scope
                     */
                    string propertyName;
                    if (propertiesNamesDeclared.ContainsKey(subfragmentProperty.getPropertyName()))
                    {
                        propertyName = this.generatePropertyName(subfragmentScript.getScriptHeader(), subfragmentProperty);
                        subfragmentProperty.renameTo(propertyName);
                    }
                    else
                    {
                        propertyName = subfragmentProperty.getPropertyName();
                    }

                    propertiesNamesDeclared[propertyName] = true;
                    resultingGlobalScope.add(subfragmentProperty);
                }

                List<ITES5CodeBlock> subfragmentBlocks = subfragmentScript.getBlockList().getBlocks();
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
                implementedStages.Add(subfragment.getStage(), true);
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

            this.mappedTargetsLogService.writeScriptName(resultingFragmentName);
            foreach (var kvp in stageMap.getMappedTargetsIndex())
            {
                var originalTargetIndex = kvp.Key;
                var mappedTargetIndexes = kvp.Value;
                this.mappedTargetsLogService.add(originalTargetIndex, mappedTargetIndexes);
            }

            TES5Script resultingTree = new TES5Script(resultingGlobalScope, resultingBlockList);
            return new TES5Target(resultingTree, outputPath);
        }

        private string generatePropertyName(TES5ScriptHeader header, TES5Property property)
        {
            string propertyName = property.getPropertyName();
            return "col_" + propertyName.Substring(0, propertyName.Length - 2) + "_" + PHPFunction.MD5(header.getScriptName()).Substring(0, 4);
        }

        private StageMap buildStageMap(BuildTarget target, string resultingFragmentName)
        {
            string sourcePath = target.getSourceFromPath(resultingFragmentName);
            string scriptName = Path.GetFileName(sourcePath);
            string stageMapFile = Path.GetDirectoryName(sourcePath) + "/" + scriptName + ".map";
            string[] stageMapFileLines = File.ReadAllLines(stageMapFile);
            Dictionary<int, List<int>> stageMap = new Dictionary<int, List<int>>();
            foreach (var stageMapLine in stageMapFileLines)
            {
                string[] e = stageMapLine.Split('-');
                int stageId = int.Parse(e[0].Trim());
                /*
                 * Clear the rows
                 */
                string[] stageRowsRaw = e[1].Split(' ');
                List<int> stageRows = new List<int>();
                foreach (var stageRowRaw in stageRowsRaw)
                {
                    string stageRowRawTrimmed = stageRowRaw.Trim();
                    if (stageRowRawTrimmed != "")
                    {
                        stageRows.Add(int.Parse(stageRowRawTrimmed));
                    }
                }

                stageMap[stageId] = stageRows;
            }

            return new StageMap(stageMap);
        }
    }
}