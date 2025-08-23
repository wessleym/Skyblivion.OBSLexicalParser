using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5Branch : ITES5CodeChunk
    {
        public TES5SubBranch MainBranch { get; }
        public TES5SubBranchList? ElseIfBranches { get; }
        public TES5ElseSubBranch? ElseBranch { get; }
        public TES5Branch(TES5SubBranch mainBranch, TES5SubBranchList? elseifBranches = null, TES5ElseSubBranch? elseBranch = null)
        {
            this.MainBranch = mainBranch;
            this.ElseIfBranches = elseifBranches;
            this.ElseBranch = elseBranch;
        }

        public IEnumerable<string> Output
        {
            get
            {
                IEnumerable<string> lines = new string[] { "" }.Concat(this.MainBranch.GetOutput("If"));
                if (this.ElseIfBranches != null)
                {
                    lines = lines.Concat(ElseIfBranches.GetElseIfOutput());
                }
                if (this.ElseBranch != null)
                {
                    lines = lines.Concat(new string[] { "" }).Concat(this.ElseBranch.Output);
                }
                lines = lines.Concat(new string[] { "EndIf" });
                return lines;
            }
        }
    }
}