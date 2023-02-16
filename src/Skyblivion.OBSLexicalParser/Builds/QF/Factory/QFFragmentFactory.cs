using Skyblivion.ESReader.Extensions;
using Skyblivion.ESReader.PHP;
using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
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

        private static int GetTES4FormID(string resultingFragmentName)
        {
            return Convert.ToInt32(resultingFragmentName.Split('_')[2], 16) - 0x01000000;
        }

        /*
        * Joins N QF subfragments into one QF fragment that can be properly binded into Skyrim VM
        * @throws ConversionException
        */
        public TES5Target JoinQFFragments(IBuildTarget target, string resultingFragmentName, List<QuestStageScript> subfragmentsTrees)
        {
            int tes4FormID = GetTES4FormID(resultingFragmentName);
            StageMap stageMap = StageMapBuilder.Build(target, resultingFragmentName, esmAnalyzer, tes4FormID);
            /*
             * We need script fragment for objective handling for each stage, so when parsing the script fragments,
             * we'll be marking them there, and intersecting this with stage.
             * This will give us an array of stages which don't have script fragment, but will need it anyways
             * for objective handling.
             */
            TES5ScriptHeader resultingScriptHeader = TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(resultingFragmentName, TES5BasicType.T_QUEST, TES5TypeFactory.TES4_Prefix, true);
            TES5GlobalScope resultingGlobalScope = new TES5GlobalScope(resultingScriptHeader);
            string[] referenceAliases = ReferenceAliasBuilder.Build(target, resultingFragmentName, esmAnalyzer, tes4FormID).ToArray();
            foreach (string propertyName in referenceAliases)
            {
                resultingGlobalScope.AddProperty(TES5PropertyFactory.ConstructWithoutFormID(propertyName, TES5BasicType.T_REFERENCEALIAS, propertyName));
            }

            List<QuestStageBlock> questStageBlocks = new List<QuestStageBlock>();
            HashSet<int> implementedStages = new HashSet<int>();
            HashSet<string> propertiesNamesDeclared = new HashSet<string>();
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
                    if (propertiesNamesDeclared.Add(subfragmentProperty.Name))
                    {
                        resultingGlobalScope.AddProperty(subfragmentProperty);
                    }
                    else
                    {
                        if (subfragmentProperty.IsPlayerRef) { continue; }
                        //WTM:  Change:  I don't think renaming these properties actually helps anything.
                        /*
                        string propertyName = GeneratePropertyName(subfragmentScript.ScriptHeader, subfragmentProperty);
                        subfragmentProperty.Rename(propertyName);
                        if (!propertiesNamesDeclared.Add(subfragmentProperty.Name))
                        {
                            throw new ConversionException(nameof(propertiesNamesDeclared) + " already contained property " + subfragmentProperty.Name + ".");
                        }
                        */
                        //WTM:  Change:  I'm trying to unify properties and include extended type declarations.
                        TES5Property existingProperty = resultingGlobalScope.GetPropertyByName(subfragmentProperty.Name);
                        if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(existingProperty.TES5Type, subfragmentProperty.TES5Type))
                        {
                            continue;
                        }
                        if (TES5InheritanceGraphAnalyzer.IsExtending(subfragmentProperty.TES5Type, existingProperty.TES5Type))
                        {
                            existingProperty.TES5Type = subfragmentProperty.TES5Type;
                            continue;
                        }
                        if (TES5InheritanceGraphAnalyzer.IsExtending(existingProperty.TES5Type, subfragmentProperty.TES5Type.NativeType))
                        {
                            subfragmentProperty.TES5Type.NativeType = existingProperty.TES5Type.NativeType;
                            existingProperty.TES5Type = subfragmentProperty.TES5Type;
                            continue;
                        }
                        throw new ConversionException("Types were not compatible for property " + subfragmentProperty.Name + ":  " + subfragmentProperty.TES5Type.Value + " should extend " + existingProperty.TES5Type.Value + " (" + existingProperty.TES5Type.NativeType.Value + ").");
                    }
                }

                IReadOnlyList<ITES5CodeBlock> subfragmentBlocks = subfragmentScript.Blocks;
                if (subfragmentBlocks.Count != 1)
                {
                    throw new ConversionException("Wrong QF fragment, actual function count: " + subfragmentBlocks.Count + "..");
                }

                ITES5CodeBlock subfragmentBlock = subfragmentBlocks[0];
                if (subfragmentBlock.FunctionScope.BlockName != TES5FragmentFactory.GetFragmentName(0))
                {
                    throw new ConversionException("Wrong QF fragment funcname, actual function name: " + subfragmentBlock.FunctionScope.BlockName + ".");
                }

                string newFragmentFunctionName = TES5FragmentFactory.GetFragmentName(subfragment.Stage, subfragment.LogIndex);

                subfragmentBlock.FunctionScope.Rename(newFragmentFunctionName);
                List<int>? stageMapOfStage = stageMap.TryGetStageTargetsMap(subfragment.Stage);
                if (stageMapOfStage != null)
                {
                    var objectiveCodeChunks = objectiveHandlingFactory.GenerateObjectiveHandling(subfragmentBlock, resultingGlobalScope, stageMapOfStage);
                    foreach (var newCodeChunk in objectiveCodeChunks)
                    {
                        subfragmentBlock.AddChunk(newCodeChunk);
                    }
                }

                questStageBlocks.Add(new QuestStageBlock(subfragment.Stage, subfragment.LogIndex, subfragmentBlock));
                implementedStages.Add(subfragment.Stage);
            }

            /*
             * Diff to find stages which we still need to mark
             */
            int[] nonDoneStages = stageMap.StageIDs.Where(stageID => !implementedStages.Contains(stageID)).ToArray();
            foreach (int nonDoneStage in nonDoneStages)
            {
                TES5FunctionCodeBlock fragment = objectiveHandlingFactory.CreateEnclosedFragment(resultingGlobalScope, nonDoneStage, stageMap.GetStageTargetsMap(nonDoneStage));
                questStageBlocks.Add(new QuestStageBlock(nonDoneStage, 0, fragment));
            }

            this.mappedTargetsLogService.WriteScriptName(resultingFragmentName);
            foreach (var kvp in stageMap.MappedTargetsIndex)
            {
                var originalTargetIndex = kvp.Key;
                var mappedTargetIndexes = kvp.Value;
                this.mappedTargetsLogService.WriteLine(originalTargetIndex, mappedTargetIndexes);
            }

            IReadOnlyList<ITES5CodeBlock> resultingBlocks = questStageBlocks.OrderBy(b => b.StageID).ThenBy(b => b.LogIndex).Select(b => b.CodeBlock).ToArray();
            TES5Script resultingTree = new TES5Script(resultingGlobalScope, resultingBlocks, true);
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
                //It worked, but I don't think the names match with what GECKFrontend generates, so I've commented it for now.
            }
            return property.OriginalName;
        }
    }
}