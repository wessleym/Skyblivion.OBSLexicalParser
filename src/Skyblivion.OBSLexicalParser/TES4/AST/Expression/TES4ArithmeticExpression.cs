using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4ArithmeticExpression : TES4BinaryExpression<TES4ArithmeticExpressionOperator>
    {
        public TES4ArithmeticExpression(ITES4Value left, TES4ArithmeticExpressionOperator op, ITES4Value right)
            : base(left, op, right)
        { }

        public object Data
        {
            get
            {
                decimal leftValue = Convert.ToDecimal(((ITES4ValueConstant)this.LeftValue).Constant);
                decimal rightValue = Convert.ToDecimal(((ITES4ValueConstant)this.RightValue).Constant);
                return Operator.Evaluate(leftValue, rightValue);
            }
        }
    }
}