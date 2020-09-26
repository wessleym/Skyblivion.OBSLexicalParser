using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5FunctionScopeFactory
    {
        public static TES5FunctionScope CreateFromBlockType(string blockType)
        {
            TES5FunctionScope functionScope = new TES5FunctionScope(blockType);
            switch (blockType)
            {
                case "OnUpdate":
                    {
                        break;
                    }

                case "OnActivate":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akActivateRef", TES5BasicType.T_OBJECTREFERENCE, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                        break;
                    }

                case "OnInit":
                    {
                        break;
                    }

                case "OnSell":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akSeller", TES5BasicType.T_ACTOR, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR })); //todo not sure about activator meaning
                        break;
                    }

                case "OnDeath":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akKiller", TES5BasicType.T_ACTOR, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                        break;
                    }

                case "OnLoad":
                    {
                        break;
                    }

                case "OnObjectEquipped":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akBaseObject", TES5BasicType.T_FORM, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.CONTAINER }));
                        functionScope.AddParameter(new TES5SignatureParameter("akReference", TES5BasicType.T_OBJECTREFERENCE, true));
                        break;
                    }

                case "OnTriggerEnter":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akActivateRef", TES5BasicType.T_OBJECTREFERENCE, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR, TES5LocalVariableParameterMeaning.CONTAINER }));
                        break;
                    }

                case "OnEquipped":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akActor", TES5BasicType.T_ACTOR, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR, TES5LocalVariableParameterMeaning.CONTAINER }));
                        break;
                    }

                case "OnUnequipped":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akActor", TES5BasicType.T_ACTOR, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                        break;
                    }

                case "OnContainerChanged":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akNewContainer", TES5BasicType.T_OBJECTREFERENCE, true));
                        functionScope.AddParameter(new TES5SignatureParameter("akOldContainer", TES5BasicType.T_OBJECTREFERENCE, true));
                        break;
                    }

                case "OnTrigger":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akActivateRef", TES5BasicType.T_OBJECTREFERENCE, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                        break;
                    }

                case "OnHit":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akAggressor", TES5BasicType.T_OBJECTREFERENCE, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                        functionScope.AddParameter(new TES5SignatureParameter("akSource", TES5BasicType.T_FORM, true));
                        functionScope.AddParameter(new TES5SignatureParameter("akProjectile", TES5BasicType.T_PROJECTILE, true));
                        functionScope.AddParameter(new TES5SignatureParameter("abPowerAttack", TES5BasicType.T_BOOL, true));
                        functionScope.AddParameter(new TES5SignatureParameter("abSneakAttack", TES5BasicType.T_BOOL, true));
                        functionScope.AddParameter(new TES5SignatureParameter("abBashAttack", TES5BasicType.T_BOOL, true));
                        functionScope.AddParameter(new TES5SignatureParameter("abHitBlocked", TES5BasicType.T_BOOL, true));
                        break;
                    }

                case "OnCombatStateChanged":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akTarget", TES5BasicType.T_ACTOR, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                        functionScope.AddParameter(new TES5SignatureParameter("aeCombatState", TES5BasicType.T_INT, true));
                        break;
                    }

                case "OnPackageStart":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akNewPackage", TES5BasicType.T_PACKAGE, true));
                        break;
                    }

                case "OnPackageDone":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akDonePackage", TES5BasicType.T_PACKAGE, true));
                        break;
                    }

                case "OnPackageEnd":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akOldPackage", TES5BasicType.T_PACKAGE, true));
                        break;
                    }

                case "OnPackageChange":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akOldPackage", TES5BasicType.T_PACKAGE, true));
                        break;
                    }

                case "OnMagicEffectApply":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akCaster", TES5BasicType.T_OBJECTREFERENCE, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                        functionScope.AddParameter(new TES5SignatureParameter("akEffect", TES5BasicType.T_MAGICEFFECT, true));
                        break;
                    }

                case "OnReset":
                    {
                        break;
                    }

                case "OnEffectStart":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akTarget", TES5BasicType.T_ACTOR, true));
                        functionScope.AddParameter(new TES5SignatureParameter("akCaster", TES5BasicType.T_ACTOR, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                        break;
                    }

                case "OnEffectFinish":
                    {
                        functionScope.AddParameter(new TES5SignatureParameter("akTarget", TES5BasicType.T_ACTOR, true));
                        functionScope.AddParameter(new TES5SignatureParameter("akCaster", TES5BasicType.T_ACTOR, true, new TES5LocalVariableParameterMeaning[] { TES5LocalVariableParameterMeaning.ACTIVATOR }));
                        break;
                    }

                default:
                    {
                        throw new ConversionException("Cannot find new block type local scope variables out of " + blockType);
                    }
            }

            return functionScope;
        }
    }
}