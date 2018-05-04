using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4ArithmeticExpression : ITES4Expression
    {
        public ITES4Value LeftValue { get; private set; }
        private TES4ArithmeticExpressionOperator arithmeticExpressionOperator;
        public ITES4Value RightValue { get; private set; }
        public TES4ArithmeticExpression(ITES4Value leftValue, TES4ArithmeticExpressionOperator op, ITES4Value rightValue)
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
                if (arithmeticExpressionOperator == TES4ArithmeticExpressionOperator.OPERATOR_EQUAL)
                {
                    return left == right;
                }
                else if (arithmeticExpressionOperator == TES4ArithmeticExpressionOperator.OPERATOR_GREATER)
                {
                    return left > right;
                }
                else if (arithmeticExpressionOperator == TES4ArithmeticExpressionOperator.OPERATOR_GREATER_OR_EQUAL)
                {
                    return left >= right;
                }
                else if (arithmeticExpressionOperator == TES4ArithmeticExpressionOperator.OPERATOR_LESS)
                {
                    return left < right;
                }
                else if (arithmeticExpressionOperator == TES4ArithmeticExpressionOperator.OPERATOR_LESS_OR_EQUAL)
                {
                    return left <= right;
                }
                else if (arithmeticExpressionOperator == TES4ArithmeticExpressionOperator.OPERATOR_NOT_EQUAL)
                {
                    return left != right;
                }
                throw new ConversionException("Unknown TES4ArithmeticExpressionOperator");
            }
        }

        public bool HasFixedValue => this.LeftValue.HasFixedValue && this.RightValue.HasFixedValue;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.LeftValue.Filter(predicate).Concat(this.RightValue.Filter(predicate)).ToArray();
        }
    }
}