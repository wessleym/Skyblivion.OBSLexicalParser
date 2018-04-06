using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4Branch : ITES4CodeChunk
    {
        private TES4SubBranch mainBranch;
        private TES4SubBranchList elseifBranches;
        private TES4ElseSubBranch elseBranch;
        public TES4Branch(TES4SubBranch mainBranch, TES4SubBranchList elseifBranches = null, TES4ElseSubBranch elseBranch = null)
        {
            this.mainBranch = mainBranch;
            this.elseifBranches = elseifBranches;
            this.elseBranch = elseBranch;
        }

        public TES4ElseSubBranch getElseBranch()
        {
            return this.elseBranch;
        }

        public TES4SubBranchList getElseifBranches()
        {
            return this.elseifBranches;
        }

        public TES4SubBranch getMainBranch()
        {
            return this.mainBranch;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = this.mainBranch.Filter(predicate);
            if (this.elseifBranches != null)
            {
                foreach (var elseifBranch in this.elseifBranches.getSubBranches())
                {
                    filtered = filtered.Concat(elseifBranch.Filter(predicate));
                }
            }

            if (this.elseBranch != null)
            {
                filtered = filtered.Concat(this.elseBranch.filter(predicate));
            }

            return filtered.ToArray();
        }
    }
}
