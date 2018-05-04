using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5ArithmeticExpression : TES5LeftOpRightExpression<TES5ArithmeticExpressionOperator>
    {
        public TES5ArithmeticExpression(ITES5Value left, TES5ArithmeticExpressionOperator op, ITES5Value right)
            : base(left, op, right)
        { }

        public override ITES5Type TES5Type
        {
            get
            {
                if (this.LeftValue.TES5Type == TES5BasicType.T_FLOAT || this.RightValue.TES5Type == TES5BasicType.T_FLOAT)
                {
                    return TES5BasicType.T_FLOAT;
                }
                return TES5BasicType.T_INT;
            }
        }
    }
}