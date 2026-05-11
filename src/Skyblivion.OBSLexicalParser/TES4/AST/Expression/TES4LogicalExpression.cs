using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    class TES4LogicalExpression : TES4BinaryExpression<TES4LogicalExpressionOperator>
    {
        public TES4LogicalExpression(ITES4Value left, TES4LogicalExpressionOperator op, ITES4Value right)
            : base(left, op, right)
        { }
    }
}