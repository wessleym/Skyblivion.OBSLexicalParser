using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators
{
    class TES5LogicalExpressionOperator
    {
        public string Name { get; private set; }
        public TES5LogicalExpressionOperator(string name)
        {
            Name = name;
        }

        public static readonly TES5LogicalExpressionOperator
            OPERATOR_OR = new TES5LogicalExpressionOperator("||"),
            OPERATOR_AND = new TES5LogicalExpressionOperator("&&");

        public static readonly TES5LogicalExpressionOperator[] all = new TES5LogicalExpressionOperator[]
        {
            OPERATOR_OR,
            OPERATOR_AND
        };

        public static TES5LogicalExpressionOperator GetFirstOrNull(string name)
        {
            return all.Where(o => o.Name == name).FirstOrDefault();
        }
    }
}