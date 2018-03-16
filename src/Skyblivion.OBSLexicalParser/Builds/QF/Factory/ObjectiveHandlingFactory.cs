using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory
{
    class ObjectiveHandlingFactory
    {
        private TES5FragmentFunctionScopeFactory fragmentFunctionScopeFactory;
        private TES5CodeScopeFactory codeScopeFactory;
        private TES5LocalScopeFactory localScopeFactory;
        private TES5ReferenceFactory referenceFactory;
        private TES5VariableAssignationFactory variableAssignationFactory;
        private TES5BranchFactory branchFactory;
        private TES5ExpressionFactory expressionFactory;
        public ObjectiveHandlingFactory(TES5FragmentFunctionScopeFactory fragmentFunctionScopeFactory, TES5CodeScopeFactory codeScopeFactory, TES5LocalScopeFactory localScopeFactory, TES5BranchFactory branchFactory, TES5VariableAssignationFactory variableAssignationFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory)
        {
            this.fragmentFunctionScopeFactory = fragmentFunctionScopeFactory;
            this.codeScopeFactory = codeScopeFactory;
            this.localScopeFactory = localScopeFactory;
            this.variableAssignationFactory = variableAssignationFactory;
            this.referenceFactory = referenceFactory;
            this.branchFactory = branchFactory;
            this.expressionFactory = expressionFactory;
        }

        /*
             *  The stage ID
         *  List of integers describing targets being enabled or disabled for given stage
        */
        public TES5FunctionCodeBlock createEnclosedFragment(TES5GlobalScope globalScope, int stageId, List<int> stageMap)
        {
            string fragmentName = "Fragment_"+stageId.ToString();
            TES5FunctionScope functionScope = this.fragmentFunctionScopeFactory.createFromFragmentType(fragmentName, TES5FragmentType.T_QF);
            TES5CodeScope codeScope = this.codeScopeFactory.createCodeScope(this.localScopeFactory.createRootScope(functionScope));
            TES5FunctionCodeBlock codeBlock = new TES5FunctionCodeBlock(new TES5VoidType(), functionScope, codeScope);
            List<ITES5CodeChunk> chunks = this.generateObjectiveHandling(codeBlock, globalScope, stageMap);
            foreach (var chunk in chunks)
            {
                codeBlock.addChunk(chunk);
            }
            return codeBlock;
        }

        public List<ITES5CodeChunk> generateObjectiveHandling(ITES5CodeBlock codeBlock, TES5GlobalScope globalScope, List<int> stageMap)
        {
            List<ITES5CodeChunk> result = new List<ITES5CodeChunk>();
            TES5LocalVariable castedToQuest = new TES5LocalVariable("__temp", TES5BasicType.T_QUEST);
            TES5Reference referenceToTemp = this.referenceFactory.createReferenceToVariable(castedToQuest);
            result.Add(variableAssignationFactory.createAssignation(referenceToTemp, this.referenceFactory.createReferenceToSelf(globalScope)));
            TES5LocalScope localScope = codeBlock.getCodeScope().getLocalScope();
            localScope.addVariable(castedToQuest);
            int i = 0;
            foreach (var stageTargetState in stageMap)
            {
                TES5Integer targetIndex = new TES5Integer(i);
                if (stageTargetState != 0)
                {
                    //Should be visible
                    TES5ObjectCallArguments displayedArguments = new TES5ObjectCallArguments();
                    displayedArguments.add(targetIndex);
                    TES5ObjectCall isObjectiveDisplayed = new TES5ObjectCall(referenceToTemp, "IsObjectiveDisplayed", displayedArguments);
                    TES5ArithmeticExpression expression = this.expressionFactory.createArithmeticExpression(isObjectiveDisplayed, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, new TES5Integer(0));
                    TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
                    arguments.add(targetIndex);
                    arguments.add(new TES5Integer(1));
                    TES5ObjectCall showTheObjective = new TES5ObjectCall(referenceToTemp, "SetObjectiveDisplayed", arguments);
                    TES5Branch branch = this.branchFactory.createSimpleBranch(expression, localScope);
                    branch.getMainBranch().getCodeScope().add(showTheObjective);
                    result.Add(branch);
                }
                else
                {
                    TES5ObjectCallArguments displayedArguments = new TES5ObjectCallArguments();
                    displayedArguments.add(targetIndex);
                    TES5ObjectCall isObjectiveDisplayed = new TES5ObjectCall(referenceToTemp, "IsObjectiveDisplayed", displayedArguments);
                    TES5ArithmeticExpression expression = this.expressionFactory.createArithmeticExpression(isObjectiveDisplayed, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, new TES5Integer(1));
                    TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
                    arguments.add(targetIndex);
                    arguments.add(new TES5Integer(1));
                    TES5ObjectCall completeTheObjective = new TES5ObjectCall(referenceToTemp, "SetObjectiveCompleted", arguments);
                    TES5Branch branch = this.branchFactory.createSimpleBranch(expression, localScope);
                    branch.getMainBranch().getCodeScope().add(completeTheObjective);
                    result.Add(branch);
                }

                ++i;
            }

            return result;
        }
    }
}