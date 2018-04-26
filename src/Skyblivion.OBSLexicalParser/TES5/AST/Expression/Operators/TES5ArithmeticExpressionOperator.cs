using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators
{
    class TES5ArithmeticExpressionOperator : TES5ExpressionOperator//WTM:  Was the name of this accidently switched with TES5ArithmeticExpressionOperator?
    {
        private TES5ArithmeticExpressionOperator(string name) : base(name) { }

        public static readonly TES5ArithmeticExpressionOperator
            OPERATOR_ADD = new TES5ArithmeticExpressionOperator("+"),
            OPERATOR_SUBSTRACT = new TES5ArithmeticExpressionOperator("-"),
            OPERATOR_MULTIPLY = new TES5ArithmeticExpressionOperator("*"),
            OPERATOR_DIVIDE = new TES5ArithmeticExpressionOperator("/");

        private static readonly TES5ArithmeticExpressionOperator[] all = new TES5ArithmeticExpressionOperator[]
        {
            OPERATOR_ADD,
            OPERATOR_SUBSTRACT,
            OPERATOR_MULTIPLY,
            OPERATOR_DIVIDE
        };

        public static TES5ArithmeticExpressionOperator GetFirstOrNull(string name)
        {
            return all.Where(o => o.Name == name).FirstOrDefault();
        }
    }
}