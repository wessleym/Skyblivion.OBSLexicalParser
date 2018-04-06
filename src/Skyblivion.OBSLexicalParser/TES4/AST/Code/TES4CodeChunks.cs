using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4CodeChunks : ITES4CodeFilterable
    {
        public List<ITES4CodeChunk> CodeChunks { get; private set; } = new List<ITES4CodeChunk>();
        public void Add(ITES4CodeChunk chunk)
        {
            this.CodeChunks.Add(chunk);
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.CodeChunks.SelectMany(c => c.Filter(predicate)).ToArray();
        }
    }
}