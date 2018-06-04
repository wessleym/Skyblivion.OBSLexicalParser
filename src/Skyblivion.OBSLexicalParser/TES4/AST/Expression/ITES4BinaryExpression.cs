using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    interface ITES4BinaryExpression : ITES4Value
    {
        ITES4Value LeftValue { get; }
        TES4ExpressionOperator Operator { get; }
        ITES4Value RightValue { get; }
    }
}