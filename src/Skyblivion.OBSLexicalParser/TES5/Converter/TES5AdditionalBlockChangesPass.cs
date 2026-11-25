using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Converter
{
    class TES5AdditionalBlockChangesPass
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        public TES5AdditionalBlockChangesPass(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
        }

        public TES5CodeScope Modify(TES4CodeBlock block, List<ITES5CodeBlock> blockList, TES5FunctionScope blockFunctionScope, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //https://cs.elderscrolls.com/index.php?title=Begin
            //WTM:  Change:  Added:  I reorganized this method and forced each event to account for the first Oblivion parameter.
            bool accountedForParameter = false;
            switch (block.BlockType.ToLower())
            {
                case "gamemode":
                case "scripteffectupdate":
                    {
                        TES5ObjectCall registerForSingleUpdate = this.objectCallFactory.CreateRegisterForSingleUpdate(codeScope, globalScope);
                        codeScope.AddChunk(registerForSingleUpdate);
                        if (globalScope.ScriptHeader.ScriptType.NativeType == TES5BasicType.T_QUEST)
                        {
                            TES5EventCodeBlock onInitBlock = TES5BlockFactory.CreateOnInit();
                            Nullable<float> timerInterval = globalScope.GetTimerIntervalIfNoQuestDelayTime(out _);
                            if (timerInterval != null)
                            {
                                TES5ReferenceFactory.CreateTimerInterval(onInitBlock.CodeScope, timerInterval.Value);
                            }
                            onInitBlock.AddChunk(registerForSingleUpdate);
                            blockList.Add(onInitBlock);
                        }
                        break;
                    }

                case "menumode":
                    {
                        TES5ComparisonExpression menuComparisonExpression = GetMenuComparisonExpression(block.BlockParameterList.Count > 0 ? int.Parse(block.BlockParameterList[0].BlockParameter) : null);
                        codeScope = SetUpIfElseChain(blockFunctionScope, codeScope, menuComparisonExpression);
                        accountedForParameter = true;//Skyrim handles menus differently than Oblivion's MenuType.
                        break;
                    }

                case "onactivate":
                    {
                        TES5EventCodeBlock onInitBlock = TES5BlockFactory.CreateOnInit();
                        TES5ObjectCall function = this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "BlockActivation");
                        onInitBlock.AddChunk(function);
                        blockList.Add(onInitBlock);
                        //WTM:  Change:  Added:  The following scripts erroneously add a parameter to their OnActivate block:
                        //MS11BradonCorpse, SE01WaitingRoomScript, SE02LoveLetterScript, SE08Xeddefen03DoorSCRIPT, SE08Xeddefen05DoorSCRIPT, SE32TombEpitaph01SCRIPT, SEHillofSuicidesSCRIPT, SEXidPuzButton1, SEXidPuzButton2, SEXidPuzButton3, SEXidPuzButton4, SEXidPuzButtonSCRIPT, SEXidPuzHungerSCRIPT, XPEbroccaCrematorySCRIPT
                        //But OnActivate does not take a parameter.  I'm trying to use the author's intended parameter instead of just ignoring it.
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akActivateRef", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }


                case "onactorequip":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, TES5LocalVariableParameterMeaning.CONTAINER, globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onadd":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akNewContainer", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onalarm":
                    {
                        //@INCONSISTENCE - We don't account for CrimeType or Criminal.
                        codeScope.AddChunk(this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Debug, "Trace", new TES5ObjectCallArguments() { new TES5String("This function does not account for OnAlarm's CrimeType or Criminal.") }));
                        ITES5Value isAlarmed = TES5ExpressionFactory.CreateTrueBooleanExpression(this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "IsAlarmed"));
                        codeScope = SetUpIfElseChain(blockFunctionScope, codeScope, isAlarmed);
                        accountedForParameter = true;
                        break;
                    }
                /*
                case "onalarm":
                {
    
                    this.skyrimGroupEventName = "onhit";
    
                    if (this.eventArgs[1] != 3) {
                        //Nothing eelse is supported really..
                        this.omit = true;
                        break;
                    }
    
                    branch = new TES4ConditionalBranch();
                    expression = new TES4Expression();
                    leftConstant = new TES4Constant("akAggressor", "ObjectReference");
                    //actionConstant        = new TES4Constant(this.eventArgs[1],"Package");
                    actionConstant = TES4Factories.createReference(this.eventArgs[2], this);
    
                    expression.left_side = leftConstant;
                    expression.right_side = actionConstant;
                    expression.comparision_operator = TES4Expression.COMPARISION_OPERATOR_EQUAL;
    
                    codeBlock = new TES4CodeBlock();
                    codeBlock.chunks = this.chunks;
    
                    branch.ifs[] = array(
                        "rawExpression" => "SCRIPT_GENERATED",
                        "expression" => expression,
                        "codeBlock" => codeBlock
                    );
                    this.chunks = new TES4ChunkContainer();
                    this.chunks.parent = this;
                    this.chunks.addChunk(branch);
    
                    break;
                }
                */

                case "ondeath":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akKiller", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "ondrop":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akOldContainer", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onequip":
                case "onunequip":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akActor", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onhit":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akAggressor", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onhitwith":
                    {
                        var firstParameter = block.BlockParameterList.FirstOrDefault();
                        if (firstParameter != null)
                        {
                            TES5LocalScope localScope = codeScope.LocalScope;
                            ITES5Referencer hitWithCriteria = this.referenceFactory.CreateReadReference(firstParameter.BlockParameter, globalScope, multipleScriptsScope, localScope);
                            bool isAmmo = TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(TES5BasicType.T_AMMO, hitWithCriteria.TES5Type);
                            if (isAmmo)
                            {
                                TES5EventCodeBlock onHitBlock = TES5BlockFactory.CreateEventCodeBlock("OnHit");
                                TES5Reference akProjectile = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(localScope.FunctionScope.GetParameter("akProjectile"));
                                TES5IfElseChain ifElseChain = TES5IfElseFactory.CreateIfElseChain(new TES5ComparisonExpression(akProjectile, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, objectCallFactory.CreateObjectCall(hitWithCriteria, "GetProjectile")), codeScope);
                                onHitBlock.AddChunk(ifElseChain);
                                codeScope = onHitBlock.CodeScope;
                            }
                            else
                            {
                                TES5Reference akSource = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(localScope.FunctionScope.GetParameter("akSource"));
                                TES5ComparisonExpression hitWithEqualsSource = TES5ExpressionFactory.CreateComparisonExpression(akSource, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, hitWithCriteria);
                                TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
                                newCodeScope.AddChunk(TES5IfElseFactory.CreateIfElseChain(hitWithEqualsSource, codeScope));
                                codeScope = newCodeScope;
                            }
                        }
                        accountedForParameter = true;
                        break;
                    }

                case "onmagiceffecthit":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akEffect", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onpackagestart":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akNewPackage", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onpackagechange":
                case "onpackagedone":
                case "onpackageend":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akOldPackage", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onsell":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akSeller", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onstartcombat":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akTarget", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "ontrigger":
                    {
                        codeScope = SetUpIfElseChain(block, codeScope, blockFunctionScope, "akActivateRef", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "ontriggeractor":
                    {
                        TES5LocalScope localScope = codeScope.LocalScope;
                        ITES5VariableOrProperty activator = localScope.GetVariableWithMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
                        TES5Reference referenceToCastedVariable = TES5LocalVariableFactory.CreateWithInitialization(TES5BasicType.T_ACTOR, "akAsActor", activator, newCodeScope);
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(referenceToCastedVariable, TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL, new TES5None());
                        TES5CodeScope outerBranchCode = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
                        outerBranchCode.LocalScope.CopyVariablesFrom(localScope);
                        var firstParameter = block.BlockParameterList.FirstOrDefault();
                        if (firstParameter != null)
                        {
                            ITES5Referencer targetActor = this.referenceFactory.CreateReadReference(firstParameter.BlockParameter, globalScope, multipleScriptsScope, localScope);
                            TES5ComparisonExpression interExpression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariableOrProperty(activator), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, targetActor);
                            outerBranchCode.AddChunk(TES5IfElseFactory.CreateIfElseChain(interExpression, codeScope));
                        }
                        else
                        {
                            outerBranchCode.AddChunks(codeScope.CodeChunks);
                        }
                        newCodeScope.AddChunk(TES5IfElseFactory.CreateIfElseChain(expression, outerBranchCode));
                        codeScope = newCodeScope;
                        accountedForParameter = true;
                        break;
                    }

                case "onload":
                case "onreset":
                case "ontriggermob":
                case "scripteffectstart":
                case "scripteffectfinish":
                    {
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(block.BlockType + " not found.");
                    }
            }
            if (!accountedForParameter)
            {
                var firstParameter = block.BlockParameterList.FirstOrDefault();
                if (firstParameter != null)
                {
                    throw new ConversionException("Parameter not accounted for in " + block.BlockType + ":  " + firstParameter.BlockParameter);
                }
            }
            return codeScope;
        }

        private static TES5CodeScope SetUpIfElseChain(TES5FunctionScope blockFunctionScope, TES5CodeScope codeScope, ITES5Value expression)
        {
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
            //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.CodeScope));//WTM:  Change:  Why serialize and then deserialize?
            TES5CodeScope outerBranchCode = codeScope;
            outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
            newCodeScope.AddChunk(TES5IfElseFactory.CreateIfElseChain(expression, outerBranchCode));
            return newCodeScope;
        }
        private TES5CodeScope SetUpIfElseChain(TES4CodeBlock block, TES5CodeScope codeScope, TES5FunctionScope blockFunctionScope, ITES5VariableOrProperty variable, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            var firstParameter = block.BlockParameterList.FirstOrDefault();
            if (firstParameter == null)
            {
                return codeScope;
            }
            TES5Reference variableReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(variable);
            TES5LocalScope localScope = codeScope.LocalScope;
            ITES5Referencer firstVariableReference = this.referenceFactory.CreateReadReference(firstParameter.BlockParameter, globalScope, multipleScriptsScope, localScope);
            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(variableReference, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, firstVariableReference);
            return SetUpIfElseChain(blockFunctionScope, codeScope, expression);
        }
        private TES5CodeScope SetUpIfElseChain(TES4CodeBlock block, TES5CodeScope codeScope, TES5FunctionScope blockFunctionScope, string variable, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return SetUpIfElseChain(block, codeScope, blockFunctionScope, codeScope.LocalScope.GetVariableOrParameter(variable), globalScope, multipleScriptsScope);
        }
        private TES5CodeScope SetUpIfElseChain(TES4CodeBlock block, TES5CodeScope codeScope, TES5FunctionScope blockFunctionScope, TES5LocalVariableParameterMeaning meaning, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return SetUpIfElseChain(block, codeScope, blockFunctionScope, codeScope.LocalScope.GetVariableWithMeaning(meaning), globalScope, multipleScriptsScope);
        }

        private TES5ComparisonExpression GetMenuComparisonExpression(Nullable<int> menuModeArgument)
        {
            string? isMenuOpenArgument =
                menuModeArgument == null ? null :
                menuModeArgument == 1 ? "MessageBoxMenu" :
                menuModeArgument == 1002 ? "InventoryMenu" :
                menuModeArgument == 1012 ? "Sleep/Wait Menu" :
                menuModeArgument == 1014 ? "Lockpicking Menu" :
                menuModeArgument == 1022 ? "MagicMenu" :
                menuModeArgument == 1023 ? "MapMenu" :
                menuModeArgument == 1030 ? "ClassMenu"/*Skyrim has no class menu, so the generated Papyrus will always return false.*/ :
                menuModeArgument == 1034 ? "PersuasionMenu"/*Skyrim has no persuasion menu, so the generated Papyrus will always return false.*/ :
                menuModeArgument == 1040 ? "Crafting Menu" :
                throw new InvalidOperationException("Unknown " + nameof(menuModeArgument) + ": " + menuModeArgument);
            TES5ObjectCall objectCall;
            if (isMenuOpenArgument == null)
            {
                objectCall = this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Utility, "IsInMenuMode");
            }
            else
            {
                objectCall = this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.UI, "IsMenuOpen", arguments: [new TES5String(isMenuOpenArgument)]);
            }
            return TES5ExpressionFactory.CreateTrueBooleanExpression(objectCall);
        }
    }
}