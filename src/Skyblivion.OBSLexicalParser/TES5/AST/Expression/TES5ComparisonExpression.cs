using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5ComparisonExpression : TES5BinaryExpression<TES5ComparisonExpressionOperator>
    {
        public TES5ComparisonExpression(ITES5Value leftValue, TES5ComparisonExpressionOperator op, ITES5Value rightValue)
            : base(leftValue, op, rightValue)
        { }

        public override ITES5Type TES5Type => TES5BasicType.T_BOOL;
    }
}