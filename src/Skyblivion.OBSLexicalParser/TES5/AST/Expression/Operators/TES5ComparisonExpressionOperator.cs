using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators
{
    class TES5ComparisonExpressionOperator : TES5ExpressionOperator
    {
        private TES5ComparisonExpressionOperator(string name) : base(name) { }

        public TES5ComparisonExpressionOperator Flip()
        {
            if (this == OPERATOR_EQUAL || this == OPERATOR_NOT_EQUAL) { return this; }
            if (this == OPERATOR_GREATER) { return OPERATOR_LESS_OR_EQUAL; }
            if (this == OPERATOR_GREATER_OR_EQUAL) { return OPERATOR_LESS; }
            if (this == OPERATOR_LESS) { return OPERATOR_GREATER_OR_EQUAL; }
            if (this == OPERATOR_LESS_OR_EQUAL) { return OPERATOR_GREATER; }
            throw new InvalidOperationException();
        }

        public static readonly TES5ComparisonExpressionOperator
            OPERATOR_EQUAL = new TES5ComparisonExpressionOperator("=="),
            OPERATOR_NOT_EQUAL = new TES5ComparisonExpressionOperator("!="),
            OPERATOR_GREATER = new TES5ComparisonExpressionOperator(">"),
            OPERATOR_GREATER_OR_EQUAL = new TES5ComparisonExpressionOperator(">="),
            OPERATOR_LESS = new TES5ComparisonExpressionOperator("<"),
            OPERATOR_LESS_OR_EQUAL = new TES5ComparisonExpressionOperator("<=");

        private static readonly TES5ComparisonExpressionOperator[] all = new TES5ComparisonExpressionOperator[]
        {
            OPERATOR_EQUAL,
            OPERATOR_NOT_EQUAL,
            OPERATOR_GREATER,
            OPERATOR_GREATER_OR_EQUAL,
            OPERATOR_LESS,
            OPERATOR_LESS_OR_EQUAL
        };

        public static TES5ComparisonExpressionOperator GetFirst(string name)
        {
            return all.Where(o => o.Name == name).First();
        }

        public static TES5ComparisonExpressionOperator GetFirstOrNull(string name)
        {
            return all.Where(o => o.Name == name).FirstOrDefault();
        }
    }
}