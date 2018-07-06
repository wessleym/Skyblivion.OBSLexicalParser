using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5LocalScopeFactory
    {
        public static TES5LocalScope CreateRootScope(TES5FunctionScope functionScope)
        {
            return new TES5LocalScope(functionScope);
        }

        public static TES5LocalScope CreateRecursiveScope(TES5LocalScope parentScope)
        {
            return new TES5LocalScope(parentScope.FunctionScope, parentScope);
        }
    }
}