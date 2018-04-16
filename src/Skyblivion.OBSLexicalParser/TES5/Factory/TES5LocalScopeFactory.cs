using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5LocalScopeFactory
    {
        public static TES5LocalScope createRootScope(TES5FunctionScope functionScope)
        {
            return new TES5LocalScope(functionScope);
        }

        public static TES5LocalScope createRecursiveScope(TES5LocalScope parentScope)
        {
            return new TES5LocalScope(parentScope.FunctionScope, parentScope);
        }
    }
}