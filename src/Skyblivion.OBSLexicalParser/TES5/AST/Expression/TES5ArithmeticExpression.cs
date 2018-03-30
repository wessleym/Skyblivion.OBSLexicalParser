using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5ArithmeticExpression : TES5LeftOpRightExpression<TES5ArithmeticExpressionOperator>
    {
        public TES5ArithmeticExpression(ITES5Value leftValue, TES5ArithmeticExpressionOperator op, ITES5Value rightValue)
            : base(leftValue, op, rightValue)
        { }

        public override ITES5Type getType()
        {
            return TES5BasicType.T_BOOL;
        }
    }
}