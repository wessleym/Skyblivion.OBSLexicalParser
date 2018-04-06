using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4LogicalExpression : ITES4Expression
    {
        private ITES4Value leftValue;
        private ITES4Value rightValue;
        private TES4LogicalExpressionOperator op;
        public TES4LogicalExpression(ITES4Value left, TES4LogicalExpressionOperator op, ITES4Value right)
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
            bool leftValue = Convert.ToBoolean(this.leftValue.getData());
            bool rightValue = Convert.ToBoolean(this.rightValue.getData());
            if (op == TES4LogicalExpressionOperator.OPERATOR_AND)
            {
                return leftValue && rightValue;
            }
            else if (op == TES4LogicalExpressionOperator.OPERATOR_OR)
            {
                return leftValue || rightValue;
            }
            throw new ConversionException("Unknown TES4LogicalExpressionOperator");
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