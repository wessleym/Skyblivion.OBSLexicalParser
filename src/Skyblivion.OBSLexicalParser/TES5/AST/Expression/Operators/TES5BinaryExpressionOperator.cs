using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators
{
    /*
     * Class TES5BinaryExpressionOperator
     * @method static TES5BinaryExpressionOperator OPERATOR_ADD()
     * @method static TES5BinaryExpressionOperator OPERATOR_SUBSTRACT()
     * @method static TES5BinaryExpressionOperator OPERATOR_MULTIPLY()
     * @method static TES5BinaryExpressionOperator OPERATOR_DIVIDE()
     * @package Ormin\OBSLexicalParser\TES5\AST\Operators
     */
    class TES5BinaryExpressionOperator
    {
        public string Name { get; private set; }
        public TES5BinaryExpressionOperator(string name)
        {
            Name = name;
        }

        public static readonly TES5BinaryExpressionOperator
            OPERATOR_ADD = new TES5BinaryExpressionOperator("+"),
            OPERATOR_SUBSTRACT = new TES5BinaryExpressionOperator("-"),
            OPERATOR_MULTIPLY = new TES5BinaryExpressionOperator("*"),
            OPERATOR_DIVIDE = new TES5BinaryExpressionOperator("/");

        private static readonly TES5BinaryExpressionOperator[] all = new TES5BinaryExpressionOperator[]
        {
            OPERATOR_ADD,
            OPERATOR_SUBSTRACT,
            OPERATOR_MULTIPLY,
            OPERATOR_DIVIDE
        };

        public static TES5BinaryExpressionOperator GetFirstOrNull(string name)
        {
            return all.Where(o => o.Name == name).FirstOrDefault();
        }
    }
}