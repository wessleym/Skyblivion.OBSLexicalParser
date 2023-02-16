using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5VariableAssignationFactory
    {
        public static TES5VariableAssignation CreateAssignation(ITES5Value reference, ITES5Value value, TES5Comment? comment = null)
        {
            return new TES5VariableAssignation(reference, value, comment);
        }
    }
}