using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5Script : ITES5Outputtable
    {
        public TES5ScriptHeader ScriptHeader { get; private set; }
        public TES5GlobalScope GlobalScope { get; private set; }
        public TES5BlockList BlockList { get; private set; }
        public TES5Script(TES5GlobalScope globalScope, TES5BlockList blockList)
        {
            this.ScriptHeader = globalScope.ScriptHeader;
            this.GlobalScope = globalScope;
            this.BlockList = blockList;
        }

        public IEnumerable<string> Output => this.ScriptHeader.Output.Concat(this.GlobalScope.Output).Concat(this.BlockList.Output);
    }
}