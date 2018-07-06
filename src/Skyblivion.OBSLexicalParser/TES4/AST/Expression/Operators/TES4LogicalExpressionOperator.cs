using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators
{
    class TES4LogicalExpressionOperator : TES4ExpressionOperator
    {
        private TES4LogicalExpressionOperator(string name)
            : base(name)
        { }

        public static readonly TES4LogicalExpressionOperator
            OPERATOR_OR = new TES4LogicalExpressionOperator("||"),
            OPERATOR_AND = new TES4LogicalExpressionOperator("&&");

        private static readonly TES4LogicalExpressionOperator[] all = new TES4LogicalExpressionOperator[]
        {
            OPERATOR_OR,
            OPERATOR_AND
        };

        public static TES4LogicalExpressionOperator GetFirst(string name)
        {
            return all.Where(o => o.Name == name).First();
        }
    }
}