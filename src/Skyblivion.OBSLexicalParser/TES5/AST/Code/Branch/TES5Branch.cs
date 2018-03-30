using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5Branch : ITES5CodeChunk
    {
        private TES5SubBranch mainBranch;
        private TES5SubBranchList elseifBranches;
        private TES5ElseSubBranch elseBranch;
        public TES5Branch(TES5SubBranch mainBranch, TES5SubBranchList elseifBranches = null, TES5ElseSubBranch elseBranch = null)
        {
            this.mainBranch = mainBranch;
            this.elseifBranches = elseifBranches;
            this.elseBranch = elseBranch;
        }

        public IEnumerable<string> output()
        {
            IEnumerable<string> lines = this.mainBranch.GetOutput("If");
            if (this.elseifBranches != null)
            {
                lines= lines.Concat(elseifBranches.GetElseIfOutput());
            }
            if (this.elseBranch != null)
            {
                lines = lines.Concat(this.elseBranch.Output());
            }
            lines = lines.Concat(new string[] { "EndIf" });
            return lines;
        }

        public TES5ElseSubBranch getElseBranch()
        {
            return this.elseBranch;
        }

        public TES5SubBranchList getElseifBranches()
        {
            return this.elseifBranches;
        }

        public TES5SubBranch getMainBranch()
        {
            return this.mainBranch;
        }
    }
}