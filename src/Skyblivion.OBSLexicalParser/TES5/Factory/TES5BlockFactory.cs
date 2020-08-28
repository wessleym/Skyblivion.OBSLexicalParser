using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5BlockFactory
    {
        private readonly TES5ChainedCodeChunkFactory codeChunkFactory;
        private readonly TES5AdditionalBlockChangesPass changesPass;
        private readonly TES5InitialBlockCodeFactory initialBlockCodeFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public TES5BlockFactory(TES5ChainedCodeChunkFactory chainedCodeChunkFactory, TES5AdditionalBlockChangesPass changesPass, TES5InitialBlockCodeFactory initialBlockCodeFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.codeChunkFactory = chainedCodeChunkFactory;
            this.changesPass = changesPass;
            this.initialBlockCodeFactory = initialBlockCodeFactory;
            this.objectCallFactory = objectCallFactory;
        }

        private static string MapBlockType(string blockType)
        {
            string newBlockType;
            switch (blockType.ToLower())
            {
                case "gamemode":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                case "onactivate":
                    {
                        newBlockType = "OnActivate";
                        break;
                    }

                case "oninit":
                    {
                        newBlockType = "OnInit";
                        break;
                    }

                case "onsell":
                    {
                        newBlockType = "OnSell";
                        break;
                    }

                case "ondeath":
                    {
                        newBlockType = "OnDeath";
                        break;
                    }

                case "onload":
                    {
                        newBlockType = "OnLoad";
                        break;
                    }

                case "onactorequip":
                    {
                        newBlockType = "OnObjectEquipped";
                        break;
                    }

                case "ontriggeractor":
                    {
                        newBlockType = "OnTriggerEnter";
                        break;
                    }

                case "onadd":
                    {
                        newBlockType = "OnContainerChanged";
                        break;
                    }

                case "onequip":
                    {
                        newBlockType = "OnEquipped";
                        break;
                    }

                case "onunequip":
                    {
                        newBlockType = "OnUnequipped";
                        break;
                    }

                case "ondrop":
                    {
                        newBlockType = "OnContainerChanged";
                        break;
                    }

                case "ontriggermob":
                    {
                        newBlockType = "OnTriggerEnter";
                        break;
                    }

                case "ontrigger":
                    {
                        newBlockType = "OnTrigger";
                        break;
                    }

                case "onhitwith":
                    {
                        newBlockType = "OnHit";
                        break;
                    }

                case "onhit":
                    {
                        newBlockType = "OnHit";
                        break;
                    }

                case "onalarm":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                case "onstartcombat":
                    {
                        newBlockType = "OnCombatStateChanged";
                        break;
                    }

                case "onpackagestart":
                    {
                        newBlockType = "OnPackageStart";
                        break;
                    }

                case "onpackagedone":
                    {
                        newBlockType = "OnPackageEnd";
                        break;
                    }

                case "onpackageend":
                    {
                        newBlockType = "OnPackageEnd";
                        break;
                    }

                case "onpackagechange":
                    {
                        newBlockType = "OnPackageChange";
                        break;
                    }

                case "onmagiceffecthit":
                    {
                        newBlockType = "OnMagicEffectApply";
                        break;
                    }

                case "onreset":
                    {
                        newBlockType = "OnReset";
                        break;
                    }

                case "scripteffectstart":
                    {
                        newBlockType = "OnEffectStart";
                        break;
                    }

                case "scripteffectupdate":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                case "scripteffectfinish":
                    {
                        newBlockType = "OnEffectFinish";
                        break;
                    }

                case "menumode":
                    {
                        newBlockType = "OnUpdate";
                        break;
                    }

                default:
                    {
                        throw new ConversionException("Cannot find new block type out of " + blockType);
                    }
            }

            return newBlockType;
        }

        //WTM:  Change:  Added
        private static readonly Dictionary<string, TES5BasicType[]?> eventNameToTypes = new Dictionary<string, TES5BasicType[]?>()
        {
            { "OnActivate", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnAnimationEvent", new TES5BasicType[] { TES5BasicType.T_ACTIVEMAGICEFFECT, TES5BasicType.T_ALIAS, TES5BasicType.T_FORM } },
            { "OnAnimationEventUnregistered", new TES5BasicType[] { TES5BasicType.T_ACTIVEMAGICEFFECT, TES5BasicType.T_ALIAS, TES5BasicType.T_FORM } },
            { "OnAttachedToCell", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnBeginState", null },
            { "OnCellAttach", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnCellDetach", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnCellLoad", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnClose", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnCombatStateChanged", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnContainerChanged", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnDeath", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnDestructionStageChanged", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnDetachedFromCell", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnDying", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnEffectFinish", new TES5BasicType[] { TES5BasicType.T_ACTIVEMAGICEFFECT } },
            { "OnEffectStart", new TES5BasicType[] { TES5BasicType.T_ACTIVEMAGICEFFECT } },
            { "OnEndState", null },
            { "OnEnterBleedout", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnEquipped", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnGainLOS", new TES5BasicType[] { TES5BasicType.T_ACTIVEMAGICEFFECT, TES5BasicType.T_ALIAS, TES5BasicType.T_FORM } },
            { "OnGetUp", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnGrab", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnHit", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnInit", null },
            { "OnItemAdded", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnItemRemoved", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnLoad", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnLocationChange", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnLockStateChanged", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnLostLOS", new TES5BasicType[] { TES5BasicType.T_ACTIVEMAGICEFFECT, TES5BasicType.T_ALIAS, TES5BasicType.T_FORM } },
            { "OnLycanthropyStateChanged", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnMagicEffectApply", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnObjectEquipped", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnObjectUnequipped", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnOpen", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnPackageChange", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnPackageEnd", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnPackageStart", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnPlayerBowShot", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnPlayerFastTravelEnd", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnPlayerLoadGame", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnRaceSwitchComplete", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnRead", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnRelease", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnReset", new TES5BasicType[] { TES5BasicType.T_ALIAS, TES5BasicType.T_OBJECTREFERENCE, TES5BasicType.T_QUEST } },
            { "OnSell", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnSit", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnSleepStart", new TES5BasicType[] { TES5BasicType.T_FORM } },
            { "OnSleepStop", new TES5BasicType[] { TES5BasicType.T_FORM } },
            { "OnSpellCast", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnStoryActivateActor", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryAddToPlayer", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryArrest", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryAssaultActor", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryBribeNPC", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryCastMagic", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryChangeLocation", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryCraftItem", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryCrimeGold", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryCure", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryDialogue", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryDiscoverDeadBody", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryEscapeJail", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryFlatterNPC", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryHello", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryIncreaseLevel", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryIncreaseSkill", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryInfection", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryIntimidateNPC", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryJail", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryKillActor", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryNewVoicePower", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryPayFine", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryPickLock", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryPlayerGetsFavor", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryRelationshipChange", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryRemoveFromPlayer", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryScript", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryServedTime", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnStoryTrespass", new TES5BasicType[] { TES5BasicType.T_QUEST } },
            { "OnTrackedStatsEvent", new TES5BasicType[] { TES5BasicType.T_FORM } },
            { "OnTranslationAlmostComplete", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnTranslationComplete", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnTranslationFailed", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnTrapHit", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnTrapHitStart", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnTrapHitStop", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnTrigger", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnTriggerEnter", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnTriggerLeave", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnUnequipped", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnUnload", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } },
            { "OnUpdate", new TES5BasicType[] { TES5BasicType.T_ACTIVEMAGICEFFECT, TES5BasicType.T_ALIAS, TES5BasicType.T_FORM } },
            { "OnUpdateGameTime", new TES5BasicType[] { TES5BasicType.T_FORM } },
            { "OnVampireFeed", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnVampirismStateChanged", new TES5BasicType[] { TES5BasicType.T_ACTOR } },
            { "OnWardHit", new TES5BasicType[] { TES5BasicType.T_OBJECTREFERENCE } }
        };
        private static void InferEventBlockContainingType(string functionBlockName, TES5GlobalScope globalScope)
        {
            TES5BasicType[]? allowedTypes = eventNameToTypes[functionBlockName];//null indicates that any type is allowed
            if (allowedTypes == null || allowedTypes.Any(t => TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(globalScope.ScriptHeader.ScriptType, t)))
            {
                return;
            }
            if (allowedTypes.Length == 1)
            {
                TES5BasicType singleAllowedType = allowedTypes[0];
                if (globalScope.ScriptHeader.ScriptType.AllowNativeTypeInference && TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(singleAllowedType, globalScope.ScriptHeader.ScriptType.NativeType))
                {
                    if (globalScope.ScriptHeader.ScriptType.NativeType != singleAllowedType)
                    {
                        globalScope.ScriptHeader.SetNativeType(singleAllowedType);
                    }
                    return;
                }
            }
            ITES5Type basicScriptType = globalScope.ScriptHeader.ScriptType.NativeType;
            bool expected = functionBlockName == "OnHit" && TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(basicScriptType, TES5BasicType.T_ACTIVEMAGICEFFECT);
            throw new ConversionTypeMismatchException("Event " + functionBlockName + " is not allowed on " + globalScope.ScriptHeader.ScriptType.Value + " (" + basicScriptType.Value + ").", expected: expected);
        }
        public static TES5EventCodeBlock CreateEventCodeBlock(TES5FunctionScope functionScope, TES5GlobalScope globalScope)
        {
            InferEventBlockContainingType(functionScope.BlockName, globalScope);
            TES5CodeScope codeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(functionScope);
            TES5EventCodeBlock newBlock = new TES5EventCodeBlock(functionScope, codeScope);
            return newBlock;
        }
        public static TES5EventCodeBlock CreateEventCodeBlock(string eventName, TES5GlobalScope globalScope)
        {
            TES5FunctionScope functionScope = new TES5FunctionScope(eventName);
            return CreateEventCodeBlock(functionScope, globalScope);
        }

        public static TES5StateCodeBlock CreateState(string name, bool auto)
        {
            TES5FunctionScope functionScope = new TES5FunctionScope(name);
            TES5CodeScope codeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(functionScope);
            TES5StateCodeBlock state = new TES5StateCodeBlock(name, auto, functionScope, codeScope);
            return state;
        }

        public TES5BlockList CreateBlock(TES4CodeBlock block, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, ref TES5EventCodeBlock? onUpdateBlockForNonQuestOrAME)
        {
            TES5BlockList blockList = new TES5BlockList();
            string blockType = block.BlockType;
            string newBlockType = MapBlockType(blockType);
            TES4CodeChunks? chunks = block.Chunks;
            if (chunks != null && chunks.Any())
            {
                TES5FunctionScope blockFunctionScope = TES5FunctionScopeFactory.CreateFromBlockType(newBlockType);
                bool onUpdateOfNonQuestOrAME = newBlockType == "OnUpdate" && globalScope.ScriptHeader.ScriptType.NativeType != TES5BasicType.T_QUEST && globalScope.ScriptHeader.ScriptType.NativeType != TES5BasicType.T_ACTIVEMAGICEFFECT;
                bool onUpdateBlockOfNonQuestOrAMEAlreadyPresent = onUpdateBlockForNonQuestOrAME != null;
                TES5BlockList? onUpdateAdditionalBlocksOfNonQuestOrAME = null;
                TES5EventCodeBlock targetEventBlock;
                TES5CodeScope targetCodeScope;
                if (onUpdateOfNonQuestOrAME)
                {
                    if (!onUpdateBlockOfNonQuestOrAMEAlreadyPresent)
                    {
                        CreateActivationStates(globalScope, out onUpdateAdditionalBlocksOfNonQuestOrAME, out targetEventBlock);
                        onUpdateBlockForNonQuestOrAME = targetEventBlock;
                    }
                    else
                    {
                        if (onUpdateBlockForNonQuestOrAME == null) { throw new NullableException(nameof(onUpdateBlockForNonQuestOrAME)); }
                        targetEventBlock = onUpdateBlockForNonQuestOrAME;
                    }
                    targetCodeScope = targetEventBlock.CodeScope;
                }
                else
                {
                    targetEventBlock = CreateEventCodeBlock(blockFunctionScope, globalScope);
                    targetCodeScope = this.initialBlockCodeFactory.AddInitialCode(globalScope, targetEventBlock);
                }
                TES5CodeScope newScope = TES5CodeScopeFactory.CreateCodeScopeRecursive(targetCodeScope.LocalScope);
                TES5CodeScope convertedCodeScope = this.ConvertTES4CodeChunksToTES5CodeScope(chunks, newScope, globalScope, multipleScriptsScope);
                TES5CodeScope modifiedCodeScope = this.changesPass.Modify(block, blockList, blockFunctionScope, convertedCodeScope, globalScope, multipleScriptsScope);
                targetEventBlock.CodeScope.LocalScope.CopyVariablesFrom(modifiedCodeScope.LocalScope);
                targetEventBlock.CodeScope.AddChunks(modifiedCodeScope.CodeChunks);
                blockList.Add(targetEventBlock);
                if (onUpdateOfNonQuestOrAME)
                {
                    if (!onUpdateBlockOfNonQuestOrAMEAlreadyPresent)
                    {
                        if (onUpdateAdditionalBlocksOfNonQuestOrAME == null) { throw new NullableException(nameof(onUpdateAdditionalBlocksOfNonQuestOrAME)); }
                        return onUpdateAdditionalBlocksOfNonQuestOrAME;
                    }
                    return new TES5BlockList();
                }
            }
            return blockList;
        }

        private TES5CodeScope ConvertTES4CodeChunksToTES5CodeScope(TES4CodeChunks chunks, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScope(codeScope.LocalScope);
            foreach (ITES4CodeChunk codeChunk in chunks)
            {
                TES5CodeChunkCollection codeChunks = this.codeChunkFactory.CreateCodeChunk(codeChunk, codeScope, globalScope, multipleScriptsScope);
                newCodeScope.AddChunk(codeChunks);
            }
            return newCodeScope;
        }

        public static TES5EventCodeBlock CreateOnInit()
        {
            TES5FunctionScope onInitFunctionScope = TES5FunctionScopeFactory.CreateFromBlockType("OnInit");
            TES5EventCodeBlock newInitBlock = new TES5EventCodeBlock(onInitFunctionScope, TES5CodeScopeFactory.CreateCodeScopeRoot(onInitFunctionScope));
            return newInitBlock;
        }

        private void CreateActiveStateBlock(TES5GlobalScope globalScope, out TES5StateCodeBlock state, out TES5EventCodeBlock onUpdate)
        {
            state = CreateState("ActiveState", false);
            TES5EventCodeBlock onBeginState = CreateEventCodeBlock("OnBeginState", globalScope);
            onBeginState.AddChunk(this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "OnUpdate"));
            state.AddBlock(onBeginState);
            onUpdate = CreateEventCodeBlock("OnUpdate", globalScope);
            state.AddBlock(onUpdate);
        }

        private static TES5StateCodeBlock CreateInactiveStateBlock()
        {
            TES5StateCodeBlock state = CreateState("InactiveState", true);
            return state;
        }

        private void CreateActivationStates(TES5GlobalScope globalScope, out TES5BlockList blocks, out TES5EventCodeBlock onUpdate)
        {
            TES5StateCodeBlock activeState;
            CreateActiveStateBlock(globalScope, out activeState, out onUpdate);
            TES5StateCodeBlock inactiveState = CreateInactiveStateBlock();
            TES5EventCodeBlock onCellAttach = CreateEventCodeBlock("OnCellAttach", globalScope);
            onCellAttach.CodeScope.AddChunk(objectCallFactory.CreateGotoState("ActiveState", globalScope));
            TES5EventCodeBlock onCellDetach = CreateEventCodeBlock("OnCellDetach", globalScope);
            onCellDetach.CodeScope.AddChunk(objectCallFactory.CreateGotoState("InactiveState", globalScope));
            blocks = new TES5BlockList() { activeState, inactiveState, onCellAttach, onCellDetach };
        }
    }
}