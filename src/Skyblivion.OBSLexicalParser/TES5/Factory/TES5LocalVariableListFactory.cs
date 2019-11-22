using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5LocalVariableListFactory
    {
        public static void CreateCodeChunk(TES4VariableDeclarationList chunk, TES5CodeScope codeScope)
        {
            foreach (TES4VariableDeclaration variable in chunk.VariableList)
            {
                TES4Type variableType = variable.VariableType;
                TES5LocalVariable property;
                if (variableType == TES4Type.T_FLOAT)
                {
                    property = new TES5LocalVariable(variable.VariableName, TES5BasicType.T_FLOAT);
                }
                else if (variableType == TES4Type.T_INT || variableType == TES4Type.T_SHORT || variableType == TES4Type.T_LONG)
                {
                    property = new TES5LocalVariable(variable.VariableName, TES5BasicType.T_INT);
                }
                else if (variableType == TES4Type.T_REF)
                {
                    //most basic one, if something from inherited class is used, we will set to the inheriting class
                    property = new TES5LocalVariable(variable.VariableName, TES5BasicType.T_FORM);
                }
                else
                {
                    throw new ConversionException("Unknown local variable declaration type.");
                }
                codeScope.LocalScope.AddVariable(property);
            }
        }
    }
}