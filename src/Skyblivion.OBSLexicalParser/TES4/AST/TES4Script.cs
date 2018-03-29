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
        private TES4ScriptHeader scriptHeader;
        private TES4VariableDeclarationList variableDeclarationList;
        private TES4BlockList blockList;
        public TES4Script(TES4ScriptHeader scriptHeader, TES4VariableDeclarationList declarationList = null, TES4BlockList blockList = null)
        {
            this.scriptHeader = scriptHeader;
            this.variableDeclarationList = declarationList;
            this.blockList = blockList;
        }

        public TES4BlockList getBlockList()
        {
            return this.blockList;
        }

        public TES4ScriptHeader getScriptHeader()
        {
            return this.scriptHeader;
        }

        public TES4VariableDeclarationList getVariableDeclarationList()
        {
            return this.variableDeclarationList;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = new ITES4CodeFilterable[] { };
            if (this.variableDeclarationList != null)
            {
                filtered = filtered.Concat(this.variableDeclarationList.filter(predicate));
            }
            if (this.blockList != null)
            {
                filtered = filtered.Concat(this.blockList.filter(predicate));
            }
            return filtered.ToArray();
        }
    }
}