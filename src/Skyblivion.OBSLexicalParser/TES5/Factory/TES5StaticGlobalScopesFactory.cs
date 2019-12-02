using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5StaticGlobalScopesFactory
    {
        private readonly ESMAnalyzer esmAnalyzer;
        public TES5StaticGlobalScopesFactory(ESMAnalyzer esmAnalyzer)
        {
            this.esmAnalyzer = esmAnalyzer;
        }

        public List<TES5GlobalScope> CreateGlobalScopes()
        {
            List<TES5GlobalScope> globalScopes = new List<TES5GlobalScope>()
            {
                new TES5GlobalScope(new TES5ScriptHeader(TES5BasicType.TES4TimerHelperName, TES5BasicType.T_QUEST, "", false, esmAnalyzer))
            };
            TES5GlobalScope globalScope = new TES5GlobalScope(new TES5ScriptHeader(TES5BasicType.TES4ContainerName, TES5BasicType.T_QUEST, "", false, esmAnalyzer));
            globalScope.AddProperty(new TES5Property("isInJail", TES5BasicType.T_BOOL, "isInJail"));
            globalScope.AddProperty(new TES5Property("isMurderer", TES5BasicType.T_BOOL, "isMurderer"));
            globalScopes.Add(globalScope);
            return globalScopes;
        }
    }
}