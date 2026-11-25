using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5StaticGlobalScopesFactory
    {
        public static List<TES5GlobalScope> CreateGlobalScopes(TES5ScriptHeaderFactory scriptHeaderFactory)
        {
            List<TES5GlobalScope> globalScopes = new List<TES5GlobalScope>()
            {
                new TES5GlobalScope(scriptHeaderFactory.GetFromCacheOrConstructByBasicType(TES5BasicType.SKYBTimerHelperName, TES5BasicType.T_QUEST, "", false))
            };
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeaderFactory.GetFromCacheOrConstructByBasicType(TES5BasicType.SKYBContainerName, TES5BasicType.T_QUEST, "", false));
            globalScopes.Add(globalScope);
            return globalScopes;
        }
    }
}