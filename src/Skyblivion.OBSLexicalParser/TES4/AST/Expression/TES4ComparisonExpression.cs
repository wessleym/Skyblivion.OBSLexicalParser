using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System;
using System.Linq;

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
                decimal left = Convert.ToDecimal(this.LeftValue.Data);
                decimal right = Convert.ToDecimal(this.RightValue.Data);
                return arithmeticExpressionOperator.Evaluate(left, right);
            }
        }

        public bool HasFixedValue => this.LeftValue.HasFixedValue && this.RightValue.HasFixedValue;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.LeftValue.Filter(predicate).Concat(this.RightValue.Filter(predicate)).ToArray();
        }
    }
}