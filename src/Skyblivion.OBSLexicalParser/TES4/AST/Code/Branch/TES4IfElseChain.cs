using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4IfElseChain : ITES4Value, ITES4CodeChunk, ITES4CodeChunkContainer
    {
        public TES4IfElseClause IfClause { get; }
        public List<TES4IfElseClause>? ElseIfClauses { get; }
        public TES4ElseClause? ElseClause { get; }
        public TES4IfElseChain(TES4IfElseClause ifClause, List<TES4IfElseClause>? elseIfClauses = null, TES4ElseClause? elseClauses = null)
        {
            this.IfClause = ifClause;
            this.ElseIfClauses = elseIfClauses;
            this.ElseClause = elseClauses;
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            IEnumerable<ITES4CodeChunk> enumerable = this.IfClause;
            if (this.ElseIfClauses != null)
            {
                enumerable = enumerable.Concat(this.ElseIfClauses.SelectMany(b => b));
            }
            if (this.ElseClause != null)
            {
                enumerable = enumerable.Concat(this.ElseClause);
            }
            return enumerable.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
