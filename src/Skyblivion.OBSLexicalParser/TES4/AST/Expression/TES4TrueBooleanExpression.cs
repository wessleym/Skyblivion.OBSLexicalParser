using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4TrueBooleanExpression : ITES4Expression
    {
        private ITES4Value value;
        public TES4TrueBooleanExpression(ITES4Value value)
        {
            this.value = value;
        }

        public ITES4Value LeftValue => this.value;

        public ITES4Value RightValue => new TES4Integer(1);

        public TES4ExpressionOperator Operator => TES4ArithmeticExpressionOperator.OPERATOR_EQUAL;

        public object Data => BoolValue;

        public bool BoolValue => (bool)this.value.Data;

        public bool HasFixedValue => this.value.HasFixedValue;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.value.Filter(predicate);
        }
    }
}
