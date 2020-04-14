using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators
{
    class TES5LogicalExpressionOperator : TES5ExpressionOperator
    {
        private TES5LogicalExpressionOperator(string name) : base(name) { }

        public static readonly TES5LogicalExpressionOperator
            OPERATOR_OR = new TES5LogicalExpressionOperator("||"),
            OPERATOR_AND = new TES5LogicalExpressionOperator("&&");

        private static readonly TES5LogicalExpressionOperator[] all = new TES5LogicalExpressionOperator[]
        {
            OPERATOR_OR,
            OPERATOR_AND
        };

        public static TES5LogicalExpressionOperator? GetFirstOrNull(string name)
        {
            return all.Where(o => o.Name == name).FirstOrDefault();
        }
    }
}