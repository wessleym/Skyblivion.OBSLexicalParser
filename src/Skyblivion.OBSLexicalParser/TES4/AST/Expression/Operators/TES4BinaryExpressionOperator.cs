using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators
{
    class TES4BinaryExpressionOperator : TES4ExpressionOperator
    {
        private TES4BinaryExpressionOperator(string name)
            : base(name)
        { }

        public static readonly TES4BinaryExpressionOperator
            OPERATOR_ADD = new TES4BinaryExpressionOperator("+"),
            OPERATOR_SUBSTRACT = new TES4BinaryExpressionOperator("-"),
            OPERATOR_MULTIPLY = new TES4BinaryExpressionOperator("*"),
            OPERATOR_DIVIDE = new TES4BinaryExpressionOperator("/");

        public static readonly TES4BinaryExpressionOperator[] all = new TES4BinaryExpressionOperator[]
        {
            OPERATOR_ADD,
            OPERATOR_SUBSTRACT,
            OPERATOR_MULTIPLY,
            OPERATOR_DIVIDE
        };

        public static TES4BinaryExpressionOperator GetFirst(string name)
        {
            return all.Where(o => o.Name == name).First();
        }
    }
}