using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetForceSneakFactory : IFunctionFactory
    {
        private TES5ObjectCallFactory objectCallFactory;
        public SetForceSneakFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCall startSneaking = this.objectCallFactory.CreateObjectCall(calledOn, "StartSneaking", multipleScriptsScope);
            if (((TES4Integer)functionArguments[0]).IntValue == 0)
            {
                //WTM:  Change:  Since StartSneaking toggles sneaking, we check if the TES4 code wants the user to stop sneaking.
                //Then, if the user is sneaking, call StartSneaking, which actually stops sneaking.
                TES5ObjectCall isSneaking = this.objectCallFactory.CreateObjectCall(calledOn, "IsSneaking", multipleScriptsScope);
                TES5ComparisonExpression playerIsNotSneaking = TES5ExpressionFactory.CreateComparisonExpression(isSneaking, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(true));
                TES5Branch branch = TES5BranchFactory.CreateSimpleBranch(playerIsNotSneaking, codeScope.LocalScope);
                branch.MainBranch.CodeScope.Add(startSneaking);
                return branch;
            }
            return startSneaking;
        }
    }
}