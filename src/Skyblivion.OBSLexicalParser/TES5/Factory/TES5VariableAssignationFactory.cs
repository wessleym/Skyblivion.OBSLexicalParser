using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5VariableAssignationFactory
    {
        public static TES5VariableAssignation CreateAssignation(ITES5Value reference, ITES5Value value, TES5Comment? comment = null)
        {
            if (reference.TES5Type == TES5BasicType.T_INT)
            {
                TES5ObjectCall? objectCall = value as TES5ObjectCall;
                if (objectCall != null && objectCall.AccessedObject.TES5Type == TES5BasicType.T_GLOBALVARIABLE && objectCall.FunctionName == "GetValue")
                {
                    objectCall.FunctionName = "GetValueInt";
                }
            }
            return new TES5VariableAssignation(reference, value, comment);
        }
    }
}