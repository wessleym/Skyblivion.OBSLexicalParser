using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4ArithmeticExpression : ITES4BinaryExpression
    {
        public ITES4Value LeftValue { get; }
        private readonly TES4ArithmeticExpressionOperator op;
        public ITES4Value RightValue { get; }
        public TES4ArithmeticExpression(ITES4Value left, TES4ArithmeticExpressionOperator op, ITES4Value right)
        {
            this.LeftValue = left;
            this.op = op;
            this.RightValue = right;
        }

        public TES4ExpressionOperator Operator => this.op;

        public object Data
        {
            get
            {
                decimal leftValue = Convert.ToDecimal(((ITES4ValueConstant)this.LeftValue).Constant);
                decimal rightValue = Convert.ToDecimal(((ITES4ValueConstant)this.RightValue).Constant);
                if (op == TES4ArithmeticExpressionOperator.OPERATOR_ADD)
                {
                    return leftValue + rightValue;
                }
                else if (op == TES4ArithmeticExpressionOperator.OPERATOR_DIVIDE)
                {
                    return leftValue / rightValue;
                }
                else if (op == TES4ArithmeticExpressionOperator.OPERATOR_MULTIPLY)
                {
                    return leftValue * rightValue;
                }
                else if (op == TES4ArithmeticExpressionOperator.OPERATOR_SUBSTRACT)
                {
                    return leftValue - rightValue;
                }
                throw new ConversionException("Unknown TES4BinaryExpressionOperator");
            }
        }
    }
}