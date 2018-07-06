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

        public static TES5CodeScope CreateCodeScopeRoot(TES5FunctionScope functionScope)
        {
            return CreateCodeScope(TES5LocalScopeFactory.CreateRootScope(functionScope));
        }

        public static TES5CodeScope CreateCodeScopeRecursive(TES5LocalScope localScope)
        {
            return CreateCodeScope(TES5LocalScopeFactory.CreateRecursiveScope(localScope));
        }
    }
}