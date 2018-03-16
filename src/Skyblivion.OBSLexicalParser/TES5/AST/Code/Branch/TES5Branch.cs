using System.Collections.Generic;

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

        public List<string> output()
        {
            List<string> codeLines = new List<string>();
            List<string> mbExpressionOutput = this.mainBranch.getExpression().output();
            string mbExpressionOutputFirst = mbExpressionOutput[0];
            codeLines.Add("If(" + mbExpressionOutputFirst + ")");
            codeLines.AddRange(this.mainBranch.getCodeScope().output());
            if (this.elseifBranches != null)
            {
                foreach (var branch in this.elseifBranches.getBranchList())
                {
                    List<string> branchExpressionOutput = branch.getExpression().output();
                    string branchExpressionOutputFirst = branchExpressionOutput[0];
                    codeLines.Add("ElseIf("+branchExpressionOutputFirst+")");
                    codeLines.AddRange(branch.getCodeScope().output());
                }
            }

            if (this.elseBranch != null)
            {
                codeLines.Add("Else");
                codeLines.AddRange(this.elseBranch.getCodeScope().output());
            }

            codeLines.Add("EndIf");
            return codeLines;
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