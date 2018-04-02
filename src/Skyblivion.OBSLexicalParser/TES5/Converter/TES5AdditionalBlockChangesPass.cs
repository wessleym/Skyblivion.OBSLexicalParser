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
        private TES5BranchFactory branchFactory;
        private TES5VariableAssignationFactory assignationFactory;
        public TES5AdditionalBlockChangesPass(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory, TES5BranchFactory branchFactory, TES5VariableAssignationFactory assignationFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
            this.branchFactory = branchFactory;
            this.assignationFactory = assignationFactory;
        }

        public const int ON_UPDATE_TICK = 1;
        public void modify(TES4CodeBlock block, TES5EventBlockList blockList, TES5EventCodeBlock newBlock, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5FunctionScope blockFunctionScope = newBlock.getFunctionScope();
            switch (block.getBlockType().ToLower())
            {
                case "gamemode":
                case "scripteffectupdate":
                    {
                        TES5FunctionScope onInitFunctionScope = TES5BlockFunctionScopeFactory.createFromBlockType("OnInit");
                        TES5EventCodeBlock newInitBlock = new TES5EventCodeBlock(onInitFunctionScope, TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(onInitFunctionScope)));
                        TES5ObjectCallArguments args = new TES5ObjectCallArguments();
                        args.add(new TES5Float(ON_UPDATE_TICK));
                        TES5ObjectCall function = this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToSelf(globalScope), "RegisterForSingleUpdate", multipleScriptsScope, args);
                        newInitBlock.addChunk(function);
                        blockList.add(newInitBlock);
                        newBlock.addChunk(function);
                        break;
                    }

                case "onactivate":
                    {
                        TES5FunctionScope onInitFunctionScope = TES5BlockFunctionScopeFactory.createFromBlockType("OnInit");
                        TES5EventCodeBlock newInitBlock = new TES5EventCodeBlock(onInitFunctionScope, TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(onInitFunctionScope)));
                        TES5ObjectCall function = this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToSelf(globalScope), "BlockActivation", multipleScriptsScope);
                        newInitBlock.addChunk(function);
                        blockList.add(newInitBlock);
                        break;
                    }

                case "onactorequip":
                    {
                        TES4BlockParameterList blockParameterList = block.getBlockParameterList();
                        if (blockParameterList == null)
                        {
                            break;
                        }

                        List<TES4BlockParameter> blockParameterListParameterList = blockParameterList.getVariableList();
                        TES4BlockParameter tesEquippedTarget = blockParameterListParameterList[0];
                        TES5LocalScope localScope = newBlock.getCodeScope().getLocalScope();
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(this.referenceFactory.createReferenceToVariable(localScope.findVariableWithMeaning(TES5LocalVariableParameterMeaning.CONTAINER)), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.getCodeScope();
                        outerBranchCode.getLocalScope().setParentScope(newCodeScope.getLocalScope());
                        newCodeScope.add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.setCodeScope(newCodeScope);
                        break;
                    }

                case "ontriggeractor":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        TES5LocalScope localScope = newBlock.getCodeScope().getLocalScope();
                        TES5LocalVariable activator = localScope.findVariableWithMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
                        TES5LocalVariable castedToActor = new TES5LocalVariable("akAsActor", TES5BasicType.T_ACTOR);
                        TES5Reference referenceToCastedVariable = this.referenceFactory.createReferenceToVariable(castedToActor);
                        TES5Reference referenceToNonCastedVariable = this.referenceFactory.createReferenceToVariable(activator);
                        TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(referenceToCastedVariable, TES5ArithmeticExpressionOperator.OPERATOR_NOT_EQUAL, new TES5None());
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        newCodeScope.getLocalScope().addVariable(castedToActor);
                        newCodeScope.add(this.assignationFactory.createAssignation(referenceToCastedVariable, referenceToNonCastedVariable));
                        TES5CodeScope outerBranchCode;
                        if (parameterList != null)
                        {
                            //NOT TESTED
                            List<TES4BlockParameter> parameterListVariableList = parameterList.getVariableList();
                            ITES5Referencer targetActor = this.referenceFactory.createReadReference(parameterListVariableList[0].getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                            TES5ArithmeticExpression interExpression = TES5ExpressionFactory.createArithmeticExpression(this.referenceFactory.createReferenceToVariable(activator), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, targetActor);
                            //TES5CodeScope interBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                            TES5CodeScope interBranchCode = newBlock.getCodeScope();
                            outerBranchCode = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                            interBranchCode.getLocalScope().setParentScope(outerBranchCode.getLocalScope());
                            outerBranchCode.add(new TES5Branch(new TES5SubBranch(interExpression, interBranchCode)));
                        }
                        else
                        {
                            //outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                            outerBranchCode = newBlock.getCodeScope();
                            outerBranchCode.getLocalScope().setParentScope(newCodeScope.getLocalScope());
                        }

                        newCodeScope.add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.setCodeScope(newCodeScope);
                        break;
                    }

                case "onadd":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        if (parameterList == null)
                        {
                            break;
                        }

                        List<TES4BlockParameter> parameterListVariableList = parameterList.getVariableList();
                        TES4BlockParameter tesEquippedTarget = parameterListVariableList[0];
                        TES5LocalScope localScope = newBlock.getCodeScope().getLocalScope();
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(this.referenceFactory.createReferenceToVariable(localScope.getVariableByName("akNewContainer")), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.getCodeScope();
                        outerBranchCode.getLocalScope().setParentScope(newCodeScope.getLocalScope());
                        newCodeScope.add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.setCodeScope(newCodeScope);
                        break;
                    }

                case "ondrop":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        if (parameterList == null)
                        {
                            break;
                        }

                        List<TES4BlockParameter> parameterListVariableList = parameterList.getVariableList();
                        TES4BlockParameter tesEquippedTarget = parameterListVariableList[0];
                        TES5LocalScope localScope = newBlock.getCodeScope().getLocalScope();
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(this.referenceFactory.createReferenceToVariable(localScope.getVariableByName("akOldContainer")), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.getCodeScope();
                        outerBranchCode.getLocalScope().setParentScope(newCodeScope.getLocalScope());
                        newCodeScope.add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.setCodeScope(newCodeScope);
                        break;
                    }

                case "onpackagestart":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        if (parameterList == null)
                        {
                            break;
                        }

                        List<TES4BlockParameter> parameterListVariableList = parameterList.getVariableList();
                        TES4BlockParameter tesEquippedTarget = parameterListVariableList[0];
                        TES5LocalScope localScope = newBlock.getCodeScope().getLocalScope();
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(this.referenceFactory.createReferenceToVariable(localScope.getVariableByName("akNewPackage")), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.getCodeScope();
                        outerBranchCode.getLocalScope().setParentScope(newCodeScope.getLocalScope());
                        newCodeScope.add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.setCodeScope(newCodeScope);
                        break;
                    }

                case "onpackagedone":
                case "onpackagechange":
                case "onpackageend":
                    {
                        TES4BlockParameterList parameterList = block.getBlockParameterList();
                        List<TES4BlockParameter> parameterListVariableList = parameterList.getVariableList();
                        TES4BlockParameter tesEquippedTarget = parameterListVariableList[0];
                        TES5LocalScope localScope = newBlock.getCodeScope().getLocalScope();
                        ITES5Referencer newContainer = this.referenceFactory.createReadReference(tesEquippedTarget.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                        TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(this.referenceFactory.createReferenceToVariable(localScope.getVariableByName("akOldPackage")), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, newContainer);
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.getCodeScope();
                        outerBranchCode.getLocalScope().setParentScope(newCodeScope.getLocalScope());
                        newCodeScope.add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.setCodeScope(newCodeScope);
                        break;
                    }

                case "onalarm":
                    {
                        //@INCONSISTENCE - We don"t account for alarm type.
                        TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToSelf(globalScope), "IsAlarmed", multipleScriptsScope), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, new TES5Bool(true));
                        TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                        //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                        TES5CodeScope outerBranchCode = newBlock.getCodeScope();
                        outerBranchCode.getLocalScope().setParentScope(newCodeScope.getLocalScope());
                        newCodeScope.add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                        newBlock.setCodeScope(newCodeScope);
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
                            List<TES4BlockParameter> parameterListVariableList = parameterList.getVariableList();
                            TES4BlockParameter equipActor = parameterListVariableList[0];
                            TES5LocalScope localScope = newBlock.getCodeScope().getLocalScope();
                            ITES5Referencer equipActorRef = this.referenceFactory.createReference(equipActor.getBlockParameter(), globalScope, multipleScriptsScope, localScope);
                            TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(this.referenceFactory.createReferenceToVariable(localScope.getVariableByName("akActor")), TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, equipActorRef);
                            TES5CodeScope newCodeScope = TES5CodeScopeFactory.createCodeScope(TES5LocalScopeFactory.createRootScope(blockFunctionScope));
                            //TES5CodeScope outerBranchCode = PHPFunction.Deserialize<TES5CodeScope>(PHPFunction.Serialize(newBlock.getCodeScope()));//WTM:  Change:  Why serialize and then deserialize?
                            TES5CodeScope outerBranchCode = newBlock.getCodeScope();
                            outerBranchCode.getLocalScope().setParentScope(newCodeScope.getLocalScope());
                            newCodeScope.add(new TES5Branch(new TES5SubBranch(expression, outerBranchCode)));
                            newBlock.setCodeScope(newCodeScope);
                        }

                        break;
                    }
            }
        }
    }
}