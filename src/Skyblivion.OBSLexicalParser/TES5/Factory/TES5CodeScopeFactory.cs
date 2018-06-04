using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5CodeScopeFactory
    {
        public static TES5CodeScope CreateCodeScope(TES5LocalScope variableScope)
        {
            return new TES5CodeScope(variableScope);
        }
    }
}