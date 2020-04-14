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
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

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
        
        public void Modify(TES4CodeBlock block, TES5BlockList blockList, TES5EventCodeBlock newBlock, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5FunctionScope blockFunctionScope = newBlock.FunctionScope;
            switch (block.BlockType.ToLower())
            {
                case "gamemode":
                case "scripteffectupdate":
                    {
                        TES5ObjectCall function = this.objectCallFactory.CreateRegisterForSingleUpdate(globalScope);
                        newBlock.AddChunk(function);
                        if (globalScope.ScriptHeader.ScriptType.NativeType == TES5BasicType.T_QUEST)
                        {
                            TES5EventCodeBlock onInitBlock = TES5BlockFactory.CreateOnInit();
                            onInitBlock.AddChunk(function);
                            blockList.Add(onInitBlock);
                        }
                        break;
                    }

                case "onactivate":
                    {
                        TES5EventCodeBlock onInitBlock = TES5BlockFactory.CreateOnInit();
                        TES5ObjectCall function = this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "BlockActivation");
                        onInitBlock.AddChunk(function);
                        blockList.Add(onInitBlock);
                        break;
                    }

                case "onactorequip":
                    {
                        SetUpBranch(block, newBlock, blockFunctionScope, TES5LocalVariableParameterMeaning.CONTAINER, globalScope, multipleScriptsScope);
                        break;
                    }

                case "ontriggeractor":
                    {
                        TES4BlockParameterList? parameterList = block.BlockParameterList;
                        TES5LocalScope localScope = newBlock.CodeScope.LocalScope;
                        ITES5VariableOrProperty activator = localScope.GetVariableWithMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
                        TES5LocalVariable castedToActor = new TES5LocalVariable("akAsActor", TES5BasicType.T_ACTOR);
                        TES5Reference referenceToCastedVariable = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(castedToActor);
                        TES5Reference referenceToNonCastedVariable = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(activator);
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(referenceToCastedVariable, TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL, new TES5None());
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
                        newCodeScope.LocalScope.AddVariable(castedToActor);
                        newCodeScope.AddChunk(TES5VariableAssignationFactory.CreateAssignation(referenceToCastedVariable, referenceToNonCastedVariable));
                        TES5CodeScope outerBranchCode;
                        if (parameterList != null)
                        {
                            //NOT TESTED
                            List<TES4BlockParameter> parameterListVariableList = parameterList.Parameters;
                            ITES5Referencer targetActor = this.referenceFactory.CreateReadReference(parameterListVariableList[0].BlockParameter, globalScope, multipleScriptsScope, localScope);
                            TES5ComparisonExpression interExpression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariableOrProperty(activator), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, targetActor);
                            //TES5CodeScope interBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.CodeScope));//WTM:  Change:  Why serialize and then deserialize?
                            TES5CodeScope interBranchCode = newBlock.CodeScope;
                            outerBranchCode = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
                            interBranchCode.LocalScope.ParentScope = outerBranchCode.LocalScope;
                            outerBranchCode.AddChunk(new TES5Branch(new TES5SubBranch(interExpression, interBranchCode)));
                        }
                        else
                        {
                            //outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.CodeScope));//WTM:  Change:  Why serialize and then deserialize?
                            outerBranchCode = newBlock.CodeScope;
                            outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
                        }

                        newCodeScope.AddChunk(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.CodeScope = newCodeScope;
                        break;
                    }

                case "onadd":
                    {
                        SetUpBranch(block, newBlock, blockFunctionScope, "akNewContainer", globalScope, multipleScriptsScope);
                        break;
                    }

                case "ondrop":
                    {
                        SetUpBranch(block, newBlock, blockFunctionScope, "akOldContainer", globalScope, multipleScriptsScope);
                        break;
                    }

                case "onpackagestart":
                    {
                        SetUpBranch(block, newBlock, blockFunctionScope, "akNewPackage", globalScope, multipleScriptsScope);
                        break;
                    }

                case "onpackagedone":
                case "onpackagechange":
                case "onpackageend":
                    {
                        SetUpBranch(block, newBlock, blockFunctionScope, "akOldPackage", globalScope, multipleScriptsScope);
                        break;
                    }

                case "onalarm":
                    {
                        //@INCONSISTENCE - We don"t account for alarm type.
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "IsAlarmed"), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(true));
                        SetUpBranch(blockFunctionScope, newBlock, expression);
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
                case "onequip":
                case "onunequip":
                    {
                        SetUpBranch(block, newBlock, blockFunctionScope, "akActor", globalScope, multipleScriptsScope);
                        break;
                    }
            }
        }

        private static void SetUpBranch(TES5FunctionScope blockFunctionScope, TES5EventCodeBlock newBlock, ITES5Value expression)
        {
            TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(blockFunctionScope);
            //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.CodeScope));//WTM:  Change:  Why serialize and then deserialize?
            TES5CodeScope outerBranchCode = newBlock.CodeScope;
            outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
            newCodeScope.AddChunk(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
            newBlock.CodeScope = newCodeScope;
        }
        private void SetUpBranch(TES4CodeBlock block, TES5EventCodeBlock newBlock, TES5FunctionScope blockFunctionScope, ITES5VariableOrProperty variable, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4BlockParameterList? parameterList = block.BlockParameterList;
            if (parameterList == null)
            {
                return;
            }
            List<TES4BlockParameter> parameterListVariableList = parameterList.Parameters;
            TES4BlockParameter tesEquippedTarget = parameterListVariableList[0];
            TES5LocalScope localScope = newBlock.CodeScope.LocalScope;
            TES5Reference variableReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(variable);
            ITES5Referencer newContainer = this.referenceFactory.CreateReadReference(tesEquippedTarget.BlockParameter, globalScope, multipleScriptsScope, localScope);
            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(variableReference, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, newContainer);
            SetUpBranch(blockFunctionScope, newBlock, expression);
        }
        private void SetUpBranch(TES4CodeBlock block, TES5EventCodeBlock newBlock, TES5FunctionScope blockFunctionScope, string variable, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            SetUpBranch(block, newBlock, blockFunctionScope, newBlock.CodeScope.LocalScope.GetVariable(variable), globalScope, multipleScriptsScope);
        }
        private void SetUpBranch(TES4CodeBlock block, TES5EventCodeBlock newBlock, TES5FunctionScope blockFunctionScope, TES5LocalVariableParameterMeaning variable, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            SetUpBranch(block, newBlock, blockFunctionScope, newBlock.CodeScope.LocalScope.GetVariableWithMeaning(variable), globalScope, multipleScriptsScope);
        }
    }
}