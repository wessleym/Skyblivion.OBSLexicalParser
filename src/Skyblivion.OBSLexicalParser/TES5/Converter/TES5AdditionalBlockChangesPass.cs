using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Converter
{
    class TES5AdditionalBlockChangesPass
    {
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ReferenceFactory referenceFactory;
        private TES5VariableAssignationFactory assignationFactory;
        public TES5AdditionalBlockChangesPass(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory, TES5VariableAssignationFactory assignationFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
            this.assignationFactory = assignationFactory;
        }

        public const int ON_UPDATE_TICK = 1;
        public void modify(TES4CodeBlock block, TES5EventBlockList blockList, TES5EventCodeBlock newBlock, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5FunctionScope blockFunctionScope = newBlock.FunctionScope;
            switch (block.getBlockType().ToLower())
            {
                case "gamemode":
                case "scripteffectupdate":
                    {
                        TES5EventCodeBlock onInitBlock = TES5BlockFunctionScopeFactory.CreateOnInit();
                        TES5ObjectCallArguments args = new TES5ObjectCallArguments();
                        args.Add(new TES5Float(ON_UPDATE_TICK));
                        TES5ObjectCall function = this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "RegisterForSingleUpdate", multipleScriptsScope, args);
                        onInitBlock.AddChunk(function);
                        blockList.add(onInitBlock);
                        newBlock.AddChunk(function);
                        break;
                    }

                case "onactivate":
                    {
                        TES5EventCodeBlock onInitBlock = TES5BlockFunctionScopeFactory.CreateOnInit();
                        TES5ObjectCall function = this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "BlockActivation", multipleScriptsScope);
                        onInitBlock.AddChunk(function);
                        blockList.add(onInitBlock);
                        break;
                    }

                case "onactorequip":
                    {
                        TES4BlockParameterList blockParameterList = block.getBlockParameterList();
                        if (blockParameterList == null)
                        {
                            break;
                        }

                        List<TES4BlockParameter> blockParameterListParameterList = blockParameterList.VariableList;
                        TES4BlockParameter tesEquippedTarget = blockParameterListParameterList[0];
                        TES5LocalScope localScope = newBlock.CodeScope.LocalScope;
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariable(localScope.GetVariableWithMeaning(TES5LocalVariableParameterMeaning.CONTAINER)), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.CodeScope;
                        outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
                        newCodeScope.Add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.CodeScope = newCodeScope;
                        break;
                    }

                case "ontriggeractor":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        TES5LocalScope localScope = newBlock.CodeScope.LocalScope;
                        TES5LocalVariable activator = localScope.GetVariableWithMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
                        TES5LocalVariable castedToActor = new TES5LocalVariable("akAsActor", TES5BasicType.T_ACTOR);
                        TES5Reference referenceToCastedVariable = TES5ReferenceFactory.CreateReferenceToVariable(castedToActor);
                        TES5Reference referenceToNonCastedVariable = TES5ReferenceFactory.CreateReferenceToVariable(activator);
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(referenceToCastedVariable, TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL, new TES5None());
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        newCodeScope.LocalScope.AddVariable(castedToActor);
                        newCodeScope.Add(this.assignationFactory.createAssignation(referenceToCastedVariable, referenceToNonCastedVariable));
                        TES5CodeScope outerBranchCode;
                        if (parameterList != null)
                        {
                            //NOT TESTED
                            List<TES4BlockParameter> parameterListVariableList = parameterList.VariableList;
                            ITES5Referencer targetActor = this.referenceFactory.createReadReference(parameterListVariableList[0].getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                            TES5ComparisonExpression interExpression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariable(activator), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, targetActor);
                            //TES5CodeScope interBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                            TES5CodeScope interBranchCode = newBlock.CodeScope;
                            outerBranchCode = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                            interBranchCode.LocalScope.ParentScope = outerBranchCode.LocalScope;
                            outerBranchCode.Add(new TES5Branch(new TES5SubBranch(interExpression, interBranchCode)));
                        }
                        else
                        {
                            //outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                            outerBranchCode = newBlock.CodeScope;
                            outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
                        }

                        newCodeScope.Add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.CodeScope = newCodeScope;
                        break;
                    }

                case "onadd":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        if (parameterList == null)
                        {
                            break;
                        }

                        List<TES4BlockParameter> parameterListVariableList = parameterList.VariableList;
                        TES4BlockParameter tesEquippedTarget = parameterListVariableList[0];
                        TES5LocalScope localScope = newBlock.CodeScope.LocalScope;
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariable(localScope.GetVariable("akNewContainer")), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.CodeScope;
                        outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
                        newCodeScope.Add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.CodeScope = newCodeScope;
                        break;
                    }

                case "ondrop":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        if (parameterList == null)
                        {
                            break;
                        }

                        List<TES4BlockParameter> parameterListVariableList = parameterList.VariableList;
                        TES4BlockParameter tesEquippedTarget = parameterListVariableList[0];
                        TES5LocalScope localScope = newBlock.CodeScope.LocalScope;
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariable(localScope.GetVariable("akOldContainer")), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.CodeScope;
                        outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
                        newCodeScope.Add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.CodeScope = newCodeScope;
                        break;
                    }

                case "onpackagestart":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        if (parameterList == null)
                        {
                            break;
                        }

                        List<TES4BlockParameter> parameterListVariableList = parameterList.VariableList;
                        TES4BlockParameter tesEquippedTarget = parameterListVariableList[0];
                        TES5LocalScope localScope = newBlock.CodeScope.LocalScope;
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariable(localScope.GetVariable("akNewPackage")), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.CodeScope;
                        outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
                        newCodeScope.Add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.CodeScope = newCodeScope;
                        break;
                    }

                case "onpackagedone":
                case "onpackagechange":
                case "onpackageend":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        List<TES4BlockParameter> parameterListVariableList = parameterList.VariableList;
                        TES4BlockParameter tesEquippedTarget = parameterListVariableList[0];
                        TES5LocalScope localScope = newBlock.CodeScope.LocalScope;
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariable(localScope.GetVariable("akOldPackage")), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.CodeScope;
                        outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
                        newCodeScope.Add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.CodeScope = newCodeScope;
                        break;
                    }

                case "onalarm":
                    {
                        //@INCONSISTENCE - We don"t account for alarm type.
                        TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "IsAlarmed", multipleScriptsScope), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(true));
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.CodeScope;
                        outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
                        newCodeScope.Add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.CodeScope = newCodeScope;
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
                        if (block.getBlockParameterList() != null)
                        {
                            TES4BlockParameterList parameterList = block.getBlockParameterList();
                            List<TES4BlockParameter> parameterListVariableList = parameterList.VariableList;
                            TES4BlockParameter equipActor = parameterListVariableList[0];
                            TES5LocalScope localScope = newBlock.CodeScope.LocalScope;
                            ITES5Referencer equipActorRef = this.referenceFactory.createReference(equipActor.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariable(localScope.GetVariable("akActor")), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, equipActorRef);
                            TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                            //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                            TES5CodeScope outerBranchCode = newBlock.CodeScope;
                            outerBranchCode.LocalScope.ParentScope = newCodeScope.LocalScope;
                            newCodeScope.Add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                            newBlock.CodeScope = newCodeScope;
                        }

                        break;
                    }
            }
        }
    }
}