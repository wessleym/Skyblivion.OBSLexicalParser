using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4Branch : ITES4Value, ITES4CodeChunk, ITES4CodeChunkContainer
    {
        public TES4SubBranch MainBranch { get; }
        public List<TES4SubBranch>? ElseifBranches { get; }
        public TES4ElseSubBranch? ElseBranch { get; }
        public TES4Branch(TES4SubBranch mainBranch, List<TES4SubBranch>? elseifBranches = null, TES4ElseSubBranch? elseBranch = null)
        {
            this.MainBranch = mainBranch;
            this.ElseifBranches = elseifBranches;
            this.ElseBranch = elseBranch;
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            IEnumerable<ITES4CodeChunk> enumerable = this.MainBranch;
            if (this.ElseifBranches != null)
            {
                enumerable = enumerable.Concat(this.ElseifBranches.SelectMany(b => b));
            }
            if (this.ElseBranch != null)
            {
                enumerable = enumerable.Concat(this.ElseBranch);
            }
            return enumerable.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
