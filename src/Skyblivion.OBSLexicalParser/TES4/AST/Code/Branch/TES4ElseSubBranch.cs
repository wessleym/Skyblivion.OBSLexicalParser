using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4ElseSubBranch : ITES4CodeFilterable
    {
        public TES4CodeChunks? CodeChunks { get; private set; }
        public TES4ElseSubBranch(TES4CodeChunks? codeChunks = null)
        {
            this.CodeChunks = codeChunks;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = new ITES4CodeFilterable[] { };
            if (this.CodeChunks != null)
            {
                foreach (var codeChunk in this.CodeChunks)
                {
                    filtered = filtered.Concat(codeChunk.Filter(predicate));
                }
            }
            return filtered.ToArray();
        }
    }
}