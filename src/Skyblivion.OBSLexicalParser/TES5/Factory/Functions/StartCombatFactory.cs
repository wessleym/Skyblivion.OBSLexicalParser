using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
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
    class StartCombatFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public StartCombatFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Added:  This is to prevent these Papyrus messages:
            /*
            Actor is dead, cannot start combat.
            Actor has no AI process, cannot start combat. [I might need to use Actor.IsAIEnabled or even Actor.EnableAI(True).]
            */
            TES5ObjectCall isDeadCall = this.objectCallFactory.CreateObjectCall(calledOn, "IsDead");
            TES5ComparisonExpression isNotDead = TES5ExpressionFactory.CreateComparisonExpression(isDeadCall, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(false));
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments newArguments = this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope);
            TES5ObjectCall startCombatCall = this.objectCallFactory.CreateObjectCall(calledOn, "StartCombat", newArguments);
            TES5Branch ifNotDeadStartCombat = TES5BranchFactory.CreateSimpleBranch(isNotDead, codeScope.LocalScope);
            ifNotDeadStartCombat.MainBranch.CodeScope.AddChunk(startCombatCall);
            return ifNotDeadStartCombat;
        }
    }
}
