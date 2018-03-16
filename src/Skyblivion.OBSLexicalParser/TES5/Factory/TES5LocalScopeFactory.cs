using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5LocalScopeFactory
    {
        public TES5LocalScope createRootScope(TES5FunctionScope functionScope)
        {
            return new TES5LocalScope(functionScope);
        }

        public TES5LocalScope createRecursiveScope(TES5LocalScope parentScope)
        {
            return new TES5LocalScope(parentScope.getFunctionScope(), parentScope);
        }
    }
}