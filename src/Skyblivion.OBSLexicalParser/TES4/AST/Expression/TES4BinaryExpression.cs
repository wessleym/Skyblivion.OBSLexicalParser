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
            decimal leftValue = Convert.ToDecimal(this.leftValue.getData());
            decimal rightValue = Convert.ToDecimal(this.rightValue.getData());
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

        public bool hasFixedValue()
        {
            return this.leftValue.hasFixedValue() && this.rightValue.hasFixedValue();
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.leftValue.filter(predicate).Concat(this.rightValue.filter(predicate)).ToArray();
        }
    }
}