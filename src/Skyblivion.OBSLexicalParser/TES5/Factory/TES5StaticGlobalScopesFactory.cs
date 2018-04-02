using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5StaticGlobalScopesFactory
    {
        public static List<TES5GlobalScope> createGlobalScopes()
        {
            List<TES5GlobalScope> globalScopes = new List<TES5GlobalScope>();
            globalScopes.Add(new TES5GlobalScope(new TES5ScriptHeader("TES4TimerHelper", "TES4TimerHelper", TES5BasicType.T_QUEST, "")));
            TES5GlobalScope globalScope = new TES5GlobalScope(new TES5ScriptHeader("TES4Container", "TES4Container", TES5BasicType.T_QUEST, ""));
            globalScope.add(new TES5Property("isInJail", TES5BasicType.T_BOOL, "isInJail"));
            globalScope.add(new TES5Property("isMurderer", TES5BasicType.T_BOOL, "isMurderer"));
            globalScopes.Add(globalScope);
            return globalScopes;
        }
    }
}