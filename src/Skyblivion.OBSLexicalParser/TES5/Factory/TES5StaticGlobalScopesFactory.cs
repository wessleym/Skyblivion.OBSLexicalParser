using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5StaticGlobalScopesFactory
    {
        public static List<TES5GlobalScope> CreateGlobalScopes()
        {
            List<TES5GlobalScope> globalScopes = new List<TES5GlobalScope>()
            {
                new TES5GlobalScope(TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(TES5BasicType.TES4TimerHelperName, TES5BasicType.T_QUEST, "", false))
            };
            TES5GlobalScope globalScope = new TES5GlobalScope(TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(TES5BasicType.TES4ContainerName, TES5BasicType.T_QUEST, "", false));
            globalScope.AddProperty(TES5PropertyFactory.ConstructWithoutFormIDs("isInJail", TES5BasicType.T_BOOL, "isInJail"));
            globalScope.AddProperty(TES5PropertyFactory.ConstructWithoutFormIDs("isMurderer", TES5BasicType.T_BOOL, "isMurderer"));
            globalScopes.Add(globalScope);
            return globalScopes;
        }
    }
}