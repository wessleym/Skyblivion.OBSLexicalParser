using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5ExpressionFactory
    {
        public static TES5LogicalExpression CreateLogicalExpression(ITES5Value left, TES5LogicalExpressionOperator op, ITES5Value right)
        {
            return new TES5LogicalExpression(left, op, right);
        }

        public static TES5ComparisonExpression CreateComparisonExpression(ITES5Value left, TES5ComparisonExpressionOperator op, ITES5Value right)
        {
            return new TES5ComparisonExpression(left, op, right);
        }

        public static TES5ComparisonExpression CreateTrueBooleanExpression(ITES5Value valueToBeTrue)
        {
            return new TES5ComparisonExpression(valueToBeTrue, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(true));
        }

        public static TES5ComparisonExpression CreateFalseBooleanExpression(ITES5Value valueToBeTrue)
        {
            return new TES5ComparisonExpression(valueToBeTrue, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(false));
        }

        public static TES5ArithmeticExpression CreateArithmeticExpression(ITES5Value left, TES5ArithmeticExpressionOperator op, ITES5Value right, bool outputParentheses = false)
        {
            return new TES5ArithmeticExpression(left, op, right, outputParentheses: outputParentheses);
        }
    }
}