using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ExpressionFactory
    {
        public TES5LogicalExpression createLogicalExpression(ITES5Value left, TES5LogicalExpressionOperator op, ITES5Value right)
        {
            return new TES5LogicalExpression(left, op, right);
        }

        public TES5ArithmeticExpression createArithmeticExpression(ITES5Value left, TES5ArithmeticExpressionOperator op, ITES5Value right)
        {
            return new TES5ArithmeticExpression(left, op, right);
        }

        public TES5TrueBooleanExpression createTrueBooleanExpression(ITES5Value valueToBeTrue)
        {
            return new TES5TrueBooleanExpression(valueToBeTrue);
        }

        public TES5BoolCastedExpression createBoolCastedExpression(ITES5Value value)
        {
            return new TES5BoolCastedExpression(value);
        }

        public TES5BinaryExpression createBinaryExpression(ITES5Value left, TES5BinaryExpressionOperator op, ITES5Value right)
        {
            return new TES5BinaryExpression(left, op, right);
        }
    }
}