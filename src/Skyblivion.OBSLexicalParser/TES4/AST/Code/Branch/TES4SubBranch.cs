using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4SubBranch : ITES4CodeChunkContainer
    {
        public ITES4Value Expression { get; }
        public TES4CodeChunks CodeChunks { get; }
        public TES4SubBranch(ITES4Value expression, TES4CodeChunks codeChunks)
        {
            this.Expression = expression;
            this.CodeChunks = codeChunks;
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            return new ITES4CodeChunk[] { this.Expression }.Concat(CodeChunks).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}