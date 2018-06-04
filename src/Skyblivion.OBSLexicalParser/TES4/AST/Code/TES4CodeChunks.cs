using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4CodeChunks : ITES4CodeFilterable, IEnumerable<ITES4CodeChunk>
    {
        private List<ITES4CodeChunk> codeChunks = new List<ITES4CodeChunk>();
        public void Add(ITES4CodeChunk chunk)
        {
            this.codeChunks.Add(chunk);
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            return codeChunks.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.codeChunks.SelectMany(c => c.Filter(predicate)).ToArray();
        }
    }
}