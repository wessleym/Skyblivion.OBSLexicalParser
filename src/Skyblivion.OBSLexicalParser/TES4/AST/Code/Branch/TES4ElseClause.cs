using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4ElseClause : ITES4CodeChunk, ITES4CodeChunkContainer
    {
        public TES4CodeChunks CodeChunks { get; }
        public TES4ElseClause(TES4CodeChunks codeChunks)
        {
            CodeChunks = codeChunks;
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            return CodeChunks.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}