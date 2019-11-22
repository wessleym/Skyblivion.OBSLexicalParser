using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetIsPlayableRaceFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetIsPlayableRaceFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Note:  I'd prefer a single method call obviously, but this is the best solution I can imagine.
            TES5ObjectCall calledOnRaceName = objectCallFactory.CreateObjectCall(objectCallFactory.CreateObjectCall(calledOn, "GetRace"), "GetName");
            string[] playableRaces = new string[] { "Altmer", "Argonian", "Bosmer", "Breton", "Dunmer", "Imperial", "Khajiit", "Nord", "Orc", "Redguard" };
            ITES5Expression? completeStatement = null;
            foreach (string race in playableRaces)
            {
                TES5ComparisonExpression newExpression = TES5ExpressionFactory.CreateComparisonExpression(calledOnRaceName, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5String(race));
                if (completeStatement == null)
                {
                    completeStatement = newExpression;
                }
                else
                {
                    completeStatement = TES5ExpressionFactory.CreateLogicalExpression(completeStatement, TES5LogicalExpressionOperator.OPERATOR_OR, newExpression);
                }
            }
            if (completeStatement == null) { throw new NullableException(nameof(completeStatement)); }
            return completeStatement;
        }
    }
}
