using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4Branch : ITES4CodeChunk
    {
        public TES4SubBranch MainBranch { get; }
        public TES4SubBranchList? ElseifBranches { get; }
        public TES4ElseSubBranch? ElseBranch { get; }
        public TES4Branch(TES4SubBranch mainBranch, TES4SubBranchList? elseifBranches = null, TES4ElseSubBranch? elseBranch = null)
        {
            this.MainBranch = mainBranch;
            this.ElseifBranches = elseifBranches;
            this.ElseBranch = elseBranch;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = this.MainBranch.Filter(predicate);
            if (this.ElseifBranches != null)
            {
                foreach (var elseifBranch in this.ElseifBranches.Branches)
                {
                    filtered = filtered.Concat(elseifBranch.Filter(predicate));
                }
            }
            if (this.ElseBranch != null)
            {
                filtered = filtered.Concat(this.ElseBranch.Filter(predicate));
            }
            return filtered.ToArray();
        }
    }
}
