using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    internal class TES4BinaryExpression<T> : ITES4BinaryExpression where T : TES4ExpressionOperator
    {
        public ITES4Value LeftValue { get; }
        public T Operator { get; }
        public ITES4Value RightValue { get; }
        public TES4BinaryExpression(ITES4Value left, T op, ITES4Value right)
        {
            this.LeftValue = left;
            this.Operator = op;
            this.RightValue = right;
        }

        TES4ExpressionOperator ITES4BinaryExpression.Operator => Operator;

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            yield return LeftValue;
            yield return RightValue;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
