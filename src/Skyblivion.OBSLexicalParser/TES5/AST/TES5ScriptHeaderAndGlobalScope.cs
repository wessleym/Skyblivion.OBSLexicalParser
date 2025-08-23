using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5ScriptHeaderAndGlobalScope
    {
        private readonly TES5ScriptHeader scriptHeader;
        private readonly TES5GlobalScope globalScope;
        public TES5ScriptHeaderAndGlobalScope(TES5ScriptHeader scriptHeader, TES5GlobalScope globalScope)
        {
            this.scriptHeader = scriptHeader;
            this.globalScope = globalScope;
        }

        public IEnumerable<string> ScriptHeaderOutput => [scriptHeader.GetOutput(globalScope.AnyConditionalProperties)];

        public IEnumerable<string> GlobalScopeOutput => globalScope.GetOutput(!scriptHeader.IsHidden);
    }
}
