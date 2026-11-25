using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4IfElseClause : ITES4CodeChunkContainer
    {
        public ITES4Value Condition { get; }
        public TES4CodeChunks CodeChunks { get; }
        public TES4IfElseClause(ITES4Value expression, TES4CodeChunks codeChunks)
        {
            this.Condition = expression;
            this.CodeChunks = codeChunks;
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            return new ITES4CodeChunk[] { this.Condition }.Concat(CodeChunks).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}