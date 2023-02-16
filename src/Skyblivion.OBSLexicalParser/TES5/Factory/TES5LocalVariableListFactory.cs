using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    //Unused
    static class TES5LocalVariableListFactory
    {
        private static TES5BasicType GetTES5Type(TES4Type tes4Type)
        {
            if (tes4Type == TES4Type.T_FLOAT)
            {
                return TES5BasicType.T_FLOAT;
            }
            if (tes4Type == TES4Type.T_INT || tes4Type == TES4Type.T_SHORT || tes4Type == TES4Type.T_LONG)
            {
                return TES5BasicType.T_INT;
            }
            if (tes4Type == TES4Type.T_REF)
            {
                //most basic one, if something from inherited class is used, we will set to the inheriting class
                return TES5BasicType.T_FORM;
            }
            throw new ConversionException("Unknown local variable declaration type.");
        }

        public static void CreateCodeChunk(IEnumerable<TES4VariableDeclaration> variableDeclarations, TES5CodeScope codeScope)
        {
            foreach (TES4VariableDeclaration variable in variableDeclarations)
            {
                TES5BasicType tes5Type = GetTES5Type(variable.VariableType);
                TES5LocalVariable tes5Variable = new TES5LocalVariable(variable.VariableName, tes5Type);
                codeScope.LocalScope.AddVariable(tes5Variable);
            }
        }
    }
}