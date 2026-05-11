using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators
{
    class TES4ArithmeticExpressionOperator : TES4ExpressionOperator
    {
        private readonly Func<decimal, decimal, decimal> func;
        private TES4ArithmeticExpressionOperator(string name, Func<decimal, decimal, decimal> func) : base(name)
        {
            this.func = func;
        }

        public decimal Evaluate(decimal left, decimal right)
        {
            return func(left, right);
        }

        public static readonly TES4ArithmeticExpressionOperator
            OPERATOR_ADD = new TES4ArithmeticExpressionOperator("+", (l, r) => l + r),
            OPERATOR_SUBSTRACT = new TES4ArithmeticExpressionOperator("-", (l, r) => l - r),
            OPERATOR_MULTIPLY = new TES4ArithmeticExpressionOperator("*", (l, r) => l * r),
            OPERATOR_DIVIDE = new TES4ArithmeticExpressionOperator("/", (l, r) => l / r);

        private static readonly TES4ArithmeticExpressionOperator[] all = new TES4ArithmeticExpressionOperator[]
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