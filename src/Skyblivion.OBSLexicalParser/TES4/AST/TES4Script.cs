using Skyblivion.OBSLexicalParser.TES4.AST.Block;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST
{
    class TES4Script : ITES4CodeFilterable
    {
        public TES4ScriptHeader ScriptHeader { get; private set; }
        public TES4VariableDeclarationList VariableDeclarationList { get; private set; }
        public TES4BlockList BlockList { get; private set; }
        public TES4Script(TES4ScriptHeader scriptHeader, TES4VariableDeclarationList declarationList = null, TES4BlockList blockList = null)
        {
            this.ScriptHeader = scriptHeader;
            this.VariableDeclarationList = declarationList;
            this.BlockList = blockList;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = new ITES4CodeFilterable[] { };
            if (this.VariableDeclarationList != null)
            {
                filtered = filtered.Concat(this.VariableDeclarationList.Filter(predicate));
            }
            if (this.BlockList != null)
            {
                filtered = filtered.Concat(this.BlockList.Filter(predicate));
            }
            return filtered.ToArray();
        }
    }
}