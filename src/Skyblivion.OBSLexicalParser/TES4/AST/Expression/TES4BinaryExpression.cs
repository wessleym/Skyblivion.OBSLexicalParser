using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4BinaryExpression : ITES4Expression
    {
        private ITES4Value leftValue;
        private TES4BinaryExpressionOperator op;
        private ITES4Value rightValue;
        public TES4BinaryExpression(ITES4Value left, TES4BinaryExpressionOperator op, ITES4Value right)
        {
            this.leftValue = left;
            this.op = op;
            this.rightValue = right;
        }

        public ITES4Value LeftValue => this.leftValue;

        public TES4ExpressionOperator Operator => this.op;

        public ITES4Value RightValue => this.rightValue;

        public object Data
        {
            get
            {
                decimal leftValue = Convert.ToDecimal(this.leftValue.Data);
                decimal rightValue = Convert.ToDecimal(this.rightValue.Data);
                if (op == TES4BinaryExpressionOperator.OPERATOR_ADD)
                {
                    return leftValue + rightValue;
                }
                else if (op == TES4BinaryExpressionOperator.OPERATOR_DIVIDE)
                {
                    return leftValue / rightValue;
                }
                else if (op == TES4BinaryExpressionOperator.OPERATOR_MULTIPLY)
                {
                    return leftValue * rightValue;
                }
                else if (op == TES4BinaryExpressionOperator.OPERATOR_SUBSTRACT)
                {
                    return leftValue - rightValue;
                }
                throw new ConversionException("Unknown TES4BinaryExpressionOperator");
            }
        }

        public bool HasFixedValue => this.leftValue.HasFixedValue && this.rightValue.HasFixedValue;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.leftValue.Filter(predicate).Concat(this.rightValue.Filter(predicate)).ToArray();
        }
    }
}