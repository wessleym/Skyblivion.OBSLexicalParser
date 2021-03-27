using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4LogicalExpression : ITES4BinaryExpression
    {
        public ITES4Value LeftValue { get; }
        private readonly TES4LogicalExpressionOperator op;
        public ITES4Value RightValue { get; }
        public TES4LogicalExpression(ITES4Value left, TES4LogicalExpressionOperator op, ITES4Value right)
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
                bool leftValue = Convert.ToBoolean(this.LeftValue.Data);
                bool rightValue = Convert.ToBoolean(this.RightValue.Data);
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
        }

        public bool HasFixedValue => this.LeftValue.HasFixedValue && this.RightValue.HasFixedValue;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.LeftValue.Filter(predicate).Concat(this.RightValue.Filter(predicate)).ToArray();
        }
    }
}