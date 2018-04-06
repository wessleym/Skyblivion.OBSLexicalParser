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
        private ITES4Value leftValue;
        private TES4ArithmeticExpressionOperator op;
        private ITES4Value rightValue;
        public TES4ArithmeticExpression(ITES4Value leftValue, TES4ArithmeticExpressionOperator op, ITES4Value rightValue)
        {
            this.leftValue = leftValue;
            this.op = op;
            this.rightValue = rightValue;
        }

        public ITES4Value getLeftValue()
        {
            return this.leftValue;
        }

        public TES4ExpressionOperator getOperator()
        {
            return this.op;
        }

        public ITES4Value getRightValue()
        {
            return this.rightValue;
        }

        public object getData()
        {
            decimal left = Convert.ToDecimal(this.leftValue.getData());
            decimal right = Convert.ToDecimal(this.rightValue.getData());
            if (op == TES4ArithmeticExpressionOperator.OPERATOR_EQUAL)
            {
                return left == right;
            }
            else if (op == TES4ArithmeticExpressionOperator.OPERATOR_GREATER)
            {
                return left > right;
            }
            else if (op == TES4ArithmeticExpressionOperator.OPERATOR_GREATER_OR_EQUAL)
            {
                return left >= right;
            }
            else if (op == TES4ArithmeticExpressionOperator.OPERATOR_LESS)
            {
                return left < right;
            }
            else if (op == TES4ArithmeticExpressionOperator.OPERATOR_LESS_OR_EQUAL)
            {
                return left <= right;
            }
            else if (op == TES4ArithmeticExpressionOperator.OPERATOR_NOT_EQUAL)
            {
                return left != right;
            }
            throw new ConversionException("Unknown TES4ArithmeticExpressionOperator");
        }

        public bool hasFixedValue()
        {
            return this.leftValue.hasFixedValue() && this.rightValue.hasFixedValue();
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.leftValue.Filter(predicate).Concat(this.rightValue.Filter(predicate)).ToArray();
        }
    }
}