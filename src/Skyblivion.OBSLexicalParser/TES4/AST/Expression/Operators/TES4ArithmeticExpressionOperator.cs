using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators
{
    class TES4ArithmeticExpressionOperator : TES4ExpressionOperator
    {
        private TES4ArithmeticExpressionOperator(string name)
            : base(name)
        { }

        public static readonly TES4ArithmeticExpressionOperator
            OPERATOR_EQUAL = new TES4ArithmeticExpressionOperator("=="),
            OPERATOR_NOT_EQUAL = new TES4ArithmeticExpressionOperator("!="),
            OPERATOR_GREATER = new TES4ArithmeticExpressionOperator(">"),
            OPERATOR_GREATER_OR_EQUAL = new TES4ArithmeticExpressionOperator(">="),
            OPERATOR_LESS = new TES4ArithmeticExpressionOperator("<"),
            OPERATOR_LESS_OR_EQUAL = new TES4ArithmeticExpressionOperator("<=");

        public static readonly TES4ArithmeticExpressionOperator[] all = new TES4ArithmeticExpressionOperator[]
        {
            OPERATOR_EQUAL,
            OPERATOR_NOT_EQUAL,
            OPERATOR_GREATER,
            OPERATOR_GREATER_OR_EQUAL,
            OPERATOR_LESS,
            OPERATOR_LESS_OR_EQUAL
        };

        public static TES4ArithmeticExpressionOperator GetFirst(string name)
        {
            return all.Where(o => o.Name == name).First();
        }
    }
}