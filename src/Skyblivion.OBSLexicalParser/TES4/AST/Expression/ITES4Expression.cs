using Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression
{
    interface ITES4Expression : ITES4Value
    {
        ITES4Value getLeftValue();
        TES4ExpressionOperator getOperator();
        ITES4Value getRightValue();
    }
}