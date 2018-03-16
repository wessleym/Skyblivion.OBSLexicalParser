using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5CodeScopeFactory
    {
        public TES5CodeScope createCodeScope(TES5LocalScope variableScope)
        {
            return new TES5CodeScope(variableScope);
        }
    }
}