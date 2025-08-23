using Skyblivion.OBSLexicalParser.TES5.AST.Property;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    internal interface ITES5ScopeWithVariablesAddable
    {
        void AddVariable(ITES5LocalVariable localVariable);
    }
}
