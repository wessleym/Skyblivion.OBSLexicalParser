using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;

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
    }
}