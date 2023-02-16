using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4ComparisonExpression : ITES4BinaryExpression
    {
        public ITES4Value LeftValue { get; }
        private readonly TES4ComparisonExpressionOperator arithmeticExpressionOperator;
        public ITES4Value RightValue { get; }
        public TES4ComparisonExpression(ITES4Value leftValue, TES4ComparisonExpressionOperator op, ITES4Value rightValue)
        {
            this.LeftValue = leftValue;
            this.arithmeticExpressionOperator = op;
            this.RightValue = rightValue;
        }

        public TES4ExpressionOperator Operator => this.arithmeticExpressionOperator;

        public object Data
        {
            get
            {
                decimal left = Convert.ToDecimal(((ITES4ValueConstant)this.LeftValue).Constant);
                decimal right = Convert.ToDecimal(((ITES4ValueConstant)this.RightValue).Constant);
                return arithmeticExpressionOperator.Evaluate(left, right);
            }
        }
    }
}