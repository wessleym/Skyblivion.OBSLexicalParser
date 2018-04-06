using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5LogicalExpression : TES5LeftOpRightExpression<TES5LogicalExpressionOperator>
    {
        public TES5LogicalExpression(ITES5Value left, TES5LogicalExpressionOperator op, ITES5Value right)
            : base(left, op, right)
        { }

        public override ITES5Type TES5Type => TES5BasicType.T_BOOL;
    }
}