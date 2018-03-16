using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4CodeChunks
    {
        private List<ITES4CodeChunk> codeChunks = new List<ITES4CodeChunk>();
        public void add(ITES4CodeChunk chunk)
        {
            this.codeChunks.Add(chunk);
        }

        public List<ITES4CodeChunk> getCodeChunks()
        {
            return this.codeChunks;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.codeChunks.SelectMany(c => c.filter(predicate)).ToArray();
        }
    }
}