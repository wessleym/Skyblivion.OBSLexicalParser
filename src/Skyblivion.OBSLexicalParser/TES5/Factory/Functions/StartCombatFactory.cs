using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
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

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Added:  This is to prevent these Papyrus messages:
            /*
            Actor is dead, cannot start combat.
            Actor has no AI process, cannot start combat. [I might need to use Actor.IsAIEnabled or even Actor.EnableAI(True).]
            */
            TES5ObjectCall isDeadCall = this.objectCallFactory.CreateObjectCall(calledOn, "IsDead");
            TES5ComparisonExpression isNotDead = TES5ExpressionFactory.CreateFalseBooleanExpression(isDeadCall);
            TES5ObjectCall startCombatCall = this.objectCallFactory.CreateObjectCall(calledOn, function, "StartCombat", objectCallArgumentsFactory, codeScope, globalScope, multipleScriptsScope);
            TES5IfElseChain ifNotDeadStartCombatChain = TES5IfElseFactory.CreateIfElseChain(isNotDead, codeScope.LocalScope);
            ifNotDeadStartCombatChain.IfClause.CodeScope.AddChunk(startCombatCall);
            return ifNotDeadStartCombatChain;
        }
    }
}
