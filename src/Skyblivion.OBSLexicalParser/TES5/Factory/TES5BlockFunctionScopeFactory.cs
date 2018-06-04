using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5BlockFunctionScopeFactory
    {
        public static TES5FunctionScope createFromBlockType(string blockType)
        {
            TES5FunctionScope localScope = new TES5FunctionScope(blockType);
            switch (blockType)
            {
                case "OnUpdate":
                {
                    break;
                }

                case "OnActivate":
                {
                    localScope.AddVariable(new TES5LocalVariable("akActivateRef", TES5BasicType.T_OBJECTREFERENCE, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                    break;
                }

                case "OnInit":
                {
                    break;
                }

                case "OnSell":
                {
                    localScope.AddVariable(new TES5LocalVariable("akSeller", TES5BasicType.T_ACTOR, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }) //todo not sure about activator meaning
                    );
                    break;
                }

                case "OnDeath":
                {
                    localScope.AddVariable(new TES5LocalVariable("akKiller", TES5BasicType.T_ACTOR, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                    break;
                }

                case "OnLoad":
                {
                    break;
                }

                case "OnObjectEquipped":
                {
                    localScope.AddVariable(new TES5LocalVariable("akBaseObject", TES5BasicType.T_FORM, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.CONTAINER }));
                    localScope.AddVariable(new TES5LocalVariable("akReference", TES5BasicType.T_OBJECTREFERENCE));
                    break;
                }

                case "OnTriggerEnter":
                {
                    localScope.AddVariable(new TES5LocalVariable("akActivateRef", TES5BasicType.T_OBJECTREFERENCE, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR, TES5LocalVariableParameterMeaning.CONTAINER }));
                    break;
                }

                case "OnEquipped":
                {
                    localScope.AddVariable(new TES5LocalVariable("akActor", TES5BasicType.T_ACTOR, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR, TES5LocalVariableParameterMeaning.CONTAINER }));
                    break;
                }

                case "OnUnequipped":
                {
                    localScope.AddVariable(new TES5LocalVariable("akActor", TES5BasicType.T_ACTOR, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                    break;
                }

                case "OnContainerChanged":
                {
                    localScope.AddVariable(new TES5LocalVariable("akNewContainer", TES5BasicType.T_OBJECTREFERENCE));
                    localScope.AddVariable(new TES5LocalVariable("akOldContainer", TES5BasicType.T_OBJECTREFERENCE));
                    break;
                }

                case "OnTrigger":
                {
                    localScope.AddVariable(new TES5LocalVariable("akActivateRef", TES5BasicType.T_OBJECTREFERENCE, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                    break;
                }

                case "OnHit":
                {
                    localScope.AddVariable(new TES5LocalVariable("akAggressor", TES5BasicType.T_OBJECTREFERENCE, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                    localScope.AddVariable(new TES5LocalVariable("akSource", TES5BasicType.T_FORM));
                    localScope.AddVariable(new TES5LocalVariable("akProjectile", TES5BasicType.T_PROJECTILE));
                    localScope.AddVariable(new TES5LocalVariable("abPowerAttack", TES5BasicType.T_BOOL));
                    localScope.AddVariable(new TES5LocalVariable("abSneakAttack", TES5BasicType.T_BOOL));
                    localScope.AddVariable(new TES5LocalVariable("abBashAttack", TES5BasicType.T_BOOL));
                    localScope.AddVariable(new TES5LocalVariable("abHitBlocked", TES5BasicType.T_BOOL));
                    break;
                }

                case "OnCombatStateChanged":
                {
                    localScope.AddVariable(new TES5LocalVariable("akTarget", TES5BasicType.T_ACTOR, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                    localScope.AddVariable(new TES5LocalVariable("aeCombatState", TES5BasicType.T_INT));
                    break;
                }

                case "OnPackageStart":
                {
                    localScope.AddVariable(new TES5LocalVariable("akNewPackage", TES5BasicType.T_PACKAGE));
                    break;
                }

                case "OnPackageDone":
                {
                    localScope.AddVariable(new TES5LocalVariable("akDonePackage", TES5BasicType.T_PACKAGE));
                    break;
                }

                case "OnPackageEnd":
                {
                    localScope.AddVariable(new TES5LocalVariable("akOldPackage", TES5BasicType.T_PACKAGE));
                    break;
                }

                case "OnPackageChange":
                {
                    localScope.AddVariable(new TES5LocalVariable("akOldPackage", TES5BasicType.T_PACKAGE));
                    break;
                }

                case "OnMagicEffectApply":
                {
                    localScope.AddVariable(new TES5LocalVariable("akCaster", TES5BasicType.T_OBJECTREFERENCE, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                    localScope.AddVariable(new TES5LocalVariable("akMagicEffect", TES5BasicType.T_MAGICEFFECT));
                    break;
                }

                case "OnReset":
                {
                    break;
                }

                case "OnEffectStart":
                {
                    localScope.AddVariable(new TES5LocalVariable("akTarget", TES5BasicType.T_ACTOR));
                    localScope.AddVariable(new TES5LocalVariable("akCaster", TES5BasicType.T_ACTOR, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                    break;
                }

                case "OnEffectFinish":
                {
                    localScope.AddVariable(new TES5LocalVariable("akTarget", TES5BasicType.T_ACTOR));
                    localScope.AddVariable(new TES5LocalVariable("akCaster", TES5BasicType.T_ACTOR, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                    break;
                }

                default:
                {
                    throw new ConversionException("Cannot find new block type local scope variables out of "+blockType);
                }
            }

            return localScope;
        }

        public static TES5EventCodeBlock CreateOnInit()
        {
            TES5FunctionScope onInitFunctionScope = createFromBlockType("OnInit");
            TES5EventCodeBlock newInitBlock = new TES5EventCodeBlock(onInitFunctionScope, TES5CodeScopeFactory.CreateCodeScope(TES5LocalScopeFactory.createRootScope(onInitFunctionScope)));
            return newInitBlock;
        }
    }
}