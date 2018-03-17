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

        public ITES4Value getLeftValue()
        {
            return this.value;
        }

        public ITES4Value getRightValue()
        {
            return new TES4Integer(1);
        }

        public TES4ExpressionOperator getOperator()
        {
            return TES4ArithmeticExpressionOperator.OPERATOR_EQUAL;
        }

        public object getData()
        {
            return BoolValue;
        }

        public bool BoolValue => (bool)this.value.getData();

        public bool hasFixedValue()
        {
            return this.value.hasFixedValue();
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.value.filter(predicate);
        }
    }
}
