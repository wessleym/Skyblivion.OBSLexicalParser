using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4ElseSubBranch
    {
        private TES4CodeChunks codeChunks;
        public TES4ElseSubBranch(TES4CodeChunks codeChunks = null)
        {
            this.codeChunks = codeChunks;
        }

        public TES4CodeChunks getCodeChunks()
        {
            return this.codeChunks;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = new ITES4CodeFilterable[] { };
            if (this.codeChunks != null)
            {
                foreach (var codeChunk in this.codeChunks.getCodeChunks())
                {
                    filtered = filtered.Concat(codeChunk.filter(predicate));
                }
            }
            return filtered.ToArray();
        }
    }
}