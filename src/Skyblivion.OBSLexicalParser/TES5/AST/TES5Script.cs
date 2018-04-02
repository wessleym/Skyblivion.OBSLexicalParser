using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5Script : ITES5Outputtable
    {
        private TES5ScriptHeader scriptHeader;
        private TES5GlobalScope globalScope;
        private TES5BlockList blockList;
        public TES5Script(TES5GlobalScope globalScope, TES5BlockList blockList = null)
        {
            this.scriptHeader = globalScope.getScriptHeader();
            this.globalScope = globalScope;
            this.blockList = blockList;
        }

        public IEnumerable<string> output()
        {
            return this.scriptHeader.output().Concat(this.globalScope.output()).Concat(this.blockList.output());
        }

        public TES5BlockList getBlockList()
        {
            return this.blockList;
        }

        public TES5ScriptHeader getScriptHeader()
        {
            return this.scriptHeader;
        }

        public TES5GlobalScope getGlobalScope()
        {
            return this.globalScope;
        }
    }
}