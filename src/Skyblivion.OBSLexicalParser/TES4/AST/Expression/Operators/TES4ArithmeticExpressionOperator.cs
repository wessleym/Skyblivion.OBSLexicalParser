using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators
{
    class TES4ArithmeticExpressionOperator : TES4ExpressionOperator
    {
        private TES4ArithmeticExpressionOperator(string name)
            : base(name)
        { }

        public static readonly TES4ArithmeticExpressionOperator
            OPERATOR_ADD = new TES4ArithmeticExpressionOperator("+"),
            OPERATOR_SUBSTRACT = new TES4ArithmeticExpressionOperator("-"),
            OPERATOR_MULTIPLY = new TES4ArithmeticExpressionOperator("*"),
            OPERATOR_DIVIDE = new TES4ArithmeticExpressionOperator("/");

        public static readonly TES4ArithmeticExpressionOperator[] all = new TES4ArithmeticExpressionOperator[]
        {
            OPERATOR_ADD,
            OPERATOR_SUBSTRACT,
            OPERATOR_MULTIPLY,
            OPERATOR_DIVIDE
        };

        public static TES4ArithmeticExpressionOperator GetFirst(string name)
        {
            return all.Where(o => o.Name == name).First();
        }
    }
}