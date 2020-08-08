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
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory
{
    class ObjectiveHandlingFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public ObjectiveHandlingFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        /*
             *  The stage ID
         *  List of integers describing targets being enabled or disabled for given stage
        */
        public TES5FunctionCodeBlock CreateEnclosedFragment(TES5GlobalScope globalScope, int stageId, List<int> stageMap)
        {
            string fragmentName = "Fragment_"+stageId.ToString();
            TES5FunctionScope functionScope = TES5FragmentFunctionScopeFactory.CreateFromFragmentType(fragmentName, TES5FragmentType.T_QF);
            TES5CodeScope codeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(functionScope);
            TES5FunctionCodeBlock codeBlock = new TES5FunctionCodeBlock(functionScope, codeScope, TES5VoidType.Instance, false, true);
            List<ITES5CodeChunk> chunks = GenerateObjectiveHandling(codeBlock, globalScope, stageMap);
            foreach (var chunk in chunks)
            {
                codeBlock.AddChunk(chunk);
            }
            return codeBlock;
        }

        public List<ITES5CodeChunk> GenerateObjectiveHandling(ITES5CodeBlock codeBlock, TES5GlobalScope globalScope, List<int> stageMap)
        {
            List<ITES5CodeChunk> result = new List<ITES5CodeChunk>();
            //WTM:  Change:
            if (!stageMap.Any()) { return result; }
            TES5LocalVariable castedToQuest = new TES5LocalVariable("__temp", TES5BasicType.T_QUEST);//WTM:  Note:  Why is this variable even necessary?
            TES5Reference referenceToTemp = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(castedToQuest);
            TES5SelfReference questSelfReference = TES5ReferenceFactory.CreateReferenceToSelf(globalScope);
            TES5VariableAssignation tempQuestAssignation = TES5VariableAssignationFactory.CreateAssignation(referenceToTemp, questSelfReference);
            result.Add(tempQuestAssignation);
            TES5LocalScope localScope = codeBlock.CodeScope.LocalScope;
            localScope.AddVariable(castedToQuest);
            int i = 0;
            foreach (var stageTargetState in stageMap)
            {
                TES5Integer targetIndex = new TES5Integer(i);
                if (stageTargetState != 0)
                {
                    //Should be visible
                    TES5ObjectCallArguments displayedArguments = new TES5ObjectCallArguments() { targetIndex };
                    TES5ObjectCall isObjectiveDisplayed = objectCallFactory.CreateObjectCall(referenceToTemp, "IsObjectiveDisplayed", displayedArguments, inference: false);
                    TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(isObjectiveDisplayed, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Integer(0));
                    TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { targetIndex, new TES5Integer(1) };
                    TES5ObjectCall showTheObjective = objectCallFactory.CreateObjectCall(referenceToTemp, "SetObjectiveDisplayed", arguments, inference: false);
                    TES5Branch branch = TES5BranchFactory.CreateSimpleBranch(expression, localScope);
                    branch.MainBranch.CodeScope.AddChunk(showTheObjective);
                    result.Add(branch);
                }
                else
                {
                    TES5ObjectCallArguments displayedArguments = new TES5ObjectCallArguments() { targetIndex };
                    TES5ObjectCall isObjectiveDisplayed = objectCallFactory.CreateObjectCall(referenceToTemp, "IsObjectiveDisplayed", displayedArguments, inference: false);
                    TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(isObjectiveDisplayed, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Integer(1));
                    TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { targetIndex, new TES5Integer(1) };
                    TES5ObjectCall completeTheObjective = objectCallFactory.CreateObjectCall(referenceToTemp, "SetObjectiveCompleted", arguments, inference: false);
                    TES5Branch branch = TES5BranchFactory.CreateSimpleBranch(expression, localScope);
                    branch.MainBranch.CodeScope.AddChunk(completeTheObjective);
                    result.Add(branch);
                }

                ++i;
            }

            return result;
        }
    }
}