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
        
        public TES5CodeScope Modify(TES4CodeBlock block, TES5BlockList blockList, TES5FunctionScope blockFunctionScope, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //https://cs.elderscrolls.com/index.php?title=Begin
            //WTM:  Change:  Added:  I reorganized this method and forced each event to account for the first Oblivion parameter.
            bool accountedForParameter = false;
            switch (block.BlockType.ToLower())
            {
                case "gamemode":
                case "scripteffectupdate":
                    {
                        TES5ObjectCall function = this.objectCallFactory.CreateRegisterForSingleUpdate(globalScope);
                        codeScope.AddChunk(function);
                        if (globalScope.ScriptHeader.ScriptType.NativeType == TES5BasicType.T_QUEST)
                        {
                            TES5EventCodeBlock onInitBlock = TES5BlockFactory.CreateOnInit();
                            onInitBlock.AddChunk(function);
                            blockList.Add(onInitBlock);
                        }
                        break;
                    }

                case "menumode":
                    {
                        TES5ComparisonExpression isInMenuModeComparisonExpression = GetIsInMenuModeComparisonExpression();
                        codeScope = SetUpBranch(blockFunctionScope, codeScope, isInMenuModeComparisonExpression);
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
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akActivateRef", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }


                case "onactorequip":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, TES5LocalVariableParameterMeaning.CONTAINER, globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onadd":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akNewContainer", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onalarm":
                    {
                        //@INCONSISTENCE - We don"t account for CrimeType or Criminal.
                        codeScope.AddChunk(this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Debug, "Trace", new TES5ObjectCallArguments() { new TES5String("This function does not account for OnAlarm's CrimeType or Criminal.") }));
                        ITES5Value isAlarmed = TES5ExpressionFactory.CreateComparisonExpression(this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "IsAlarmed"), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(true));
                        codeScope = SetUpBranch(blockFunctionScope, codeScope, isAlarmed);
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
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akKiller", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "ondrop":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akOldContainer", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onequip":
                case "onunequip":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akActor", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onhit":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akAggressor", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onhitwith":
                    {
                        TES4BlockParameterList? parameterList = block.BlockParameterList;
                        if (parameterList != null)
                        {
                            TES5LocalScope localScope = codeScope.LocalScope;
                            ITES5Referencer hitWithCriteria = this.referenceFactory.CreateReadReference(parameterList.Parameters[0].BlockParameter, globalScope, multipleScriptsScope, localScope);
                            TES5SignatureParameter akSource = localScope.FunctionScope.GetParameter("akSource");
                            TES5ComparisonExpression hitWithEqualsSource = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariableOrProperty(akSource), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, hitWithCriteria);
                            TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
                            if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(TES5BasicType.T_AMMO, hitWithCriteria.TES5Type))
                            {
                                newCodeScope.AddChunk(this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Debug, "Trace", new TES5ObjectCallArguments() { new TES5String("OBScript called OnHitWith using Ammo, but it's unlikely Papyrus will handle it properly.  When arrows are used, " + akSource.Name + " will be a bow.") }));
                            }
                            newCodeScope.AddChunk(new TES5Branch(new TES5SubBranch(hitWithEqualsSource, codeScope)));
                            codeScope = newCodeScope;
                        }
                        accountedForParameter = true;
                        break;
                    }

                case "onmagiceffecthit":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akEffect", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onpackagestart":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akNewPackage", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onpackagechange":
                case "onpackagedone":
                case "onpackageend":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akOldPackage", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onsell":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akSeller", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "onstartcombat":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akTarget", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "ontrigger":
                    {
                        codeScope = SetUpBranch(block, codeScope, blockFunctionScope, "akActivateRef", globalScope, multipleScriptsScope);
                        accountedForParameter = true;
                        break;
                    }

                case "ontriggeractor":
                    {
                        TES4BlockParameterList? parameterList = block.BlockParameterList;
                        TES5LocalScope localScope = codeScope.LocalScope;
                        ITES5VariableOrProperty activator = localScope.GetVariableWithMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
                        TES5LocalVariable castedToActor = new TES5LocalVariable("akAsActor", TES5BasicType.T_ACTOR);
                        TES5Reference referenceToCastedVariable = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(castedToActor);
                        TES5Reference referenceToNonCastedVariable = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(activator);
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(referenceToCastedVariable, TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL, new TES5None());
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
                        newCodeScope.AddVariable(castedToActor);
                        newCodeScope.AddChunk(TES5VariableAssignationFactory.CreateAssignation(referenceToCastedVariable, referenceToNonCastedVariable));
                        TES5CodeScope outerBranchCode = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
                        outerBranchCode.LocalScope.CopyVariablesFrom(codeScope.LocalScope);
                        if (parameterList != null)
                        {
                            ITES5Referencer targetActor = this.referenceFactory.CreateReadReference(parameterList.Parameters[0].BlockParameter, globalScope, multipleScriptsScope, localScope);
                            TES5ComparisonExpression interExpression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariableOrProperty(activator), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, targetActor);
                            outerBranchCode.AddChunk(new TES5Branch(new TES5SubBranch(interExpression, codeScope)));
                        }
                        else
                        {
                            outerBranchCode.AddChunks(codeScope.CodeChunks);
                        }
                        newCodeScope.AddChunk(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
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
                TES4BlockParameterList? parameterList = block.BlockParameterList;
                if (parameterList != null && parameterList.Parameters.Any())
                {
                    throw new ConversionException("Parameter not accounted for in " + block.BlockType + ":  " + parameterList.Parameters[0].BlockParameter);
                }
            }
            return codeScope;
        }

        private static TES5CodeScope SetUpBranch(TES5FunctionScope blockFunctionScope, TES5CodeScope codeScope, ITES5Value expression)
        {
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
            //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.CodeScope));//WTM:  Change:  Why serialize and then deserialize?
            TES5CodeScope outerBranchCode = codeScope;
            outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
            newCodeScope.AddChunk(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
            return newCodeScope;
        }
        private TES5CodeScope SetUpBranch(TES4CodeBlock block, TES5CodeScope codeScope, TES5FunctionScope blockFunctionScope, ITES5VariableOrProperty variable, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4BlockParameterList? parameterList = block.BlockParameterList;
            if (parameterList == null)
            {
                return codeScope;
            }
            TES5Reference variableReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(variable);
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4BlockParameter firstParameter = parameterList.Parameters[0];
            ITES5Referencer firstVariableReference = this.referenceFactory.CreateReadReference(firstParameter.BlockParameter, globalScope, multipleScriptsScope, localScope);
            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(variableReference, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, firstVariableReference);
            return SetUpBranch(blockFunctionScope, codeScope, expression);
        }
        private TES5CodeScope SetUpBranch(TES4CodeBlock block, TES5CodeScope codeScope, TES5FunctionScope blockFunctionScope, string variable, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return SetUpBranch(block, codeScope, blockFunctionScope, codeScope.LocalScope.GetVariable(variable), globalScope, multipleScriptsScope);
        }
        private TES5CodeScope SetUpBranch(TES4CodeBlock block, TES5CodeScope codeScope, TES5FunctionScope blockFunctionScope, TES5LocalVariableParameterMeaning meaning, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return SetUpBranch(block, codeScope, blockFunctionScope, codeScope.LocalScope.GetVariableWithMeaning(meaning), globalScope, multipleScriptsScope);
        }

        private TES5ComparisonExpression GetIsInMenuModeComparisonExpression()
        {
            return TES5ExpressionFactory.CreateComparisonExpression(this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Utility, "IsInMenuMode"), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(true));
        }
    }
}