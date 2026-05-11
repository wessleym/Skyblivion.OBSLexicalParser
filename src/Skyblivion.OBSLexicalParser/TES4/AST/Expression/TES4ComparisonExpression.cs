using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4ComparisonExpression : TES4BinaryExpression<TES4ComparisonExpressionOperator>
    {
        public TES4ComparisonExpression(ITES4Value leftValue, TES4ComparisonExpressionOperator op, ITES4Value rightValue)
            : base(leftValue, op, rightValue)
        { }

        public object Data
        {
            get
            {
                decimal left = Convert.ToDecimal(((ITES4ValueConstant)this.LeftValue).Constant);
                decimal right = Convert.ToDecimal(((ITES4ValueConstant)this.RightValue).Constant);
                return Operator.Evaluate(left, right);
            }
        }
    }
}