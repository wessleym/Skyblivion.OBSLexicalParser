using System;
using System.Collections.Generic;
using System.Linq;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5Branch : ITES5ValueCodeChunk//WTM:  Note:  This class needs ITES5ValueCodeChunk instead of just ITES5CodeChunk for SetForceSteakFactory.
    {
        public TES5SubBranch MainBranch { get; private set; }
        public TES5SubBranchList ElseIfBranches { get; private set; }
        public TES5ElseSubBranch ElseBranch { get; private set; }
        public TES5Branch(TES5SubBranch mainBranch, TES5SubBranchList elseifBranches = null, TES5ElseSubBranch elseBranch = null)
        {
            this.MainBranch = mainBranch;
            this.ElseIfBranches = elseifBranches;
            this.ElseBranch = elseBranch;
        }

        public IEnumerable<string> Output
        {
            get
            {
                IEnumerable<string> lines = this.MainBranch.GetOutput("If");
                if (this.ElseIfBranches != null)
                {
                    lines = lines.Concat(ElseIfBranches.GetElseIfOutput());
                }
                if (this.ElseBranch != null)
                {
                    lines = lines.Concat(this.ElseBranch.Output());
                }
                lines = lines.Concat(new string[] { "EndIf" });
                return lines;
            }
        }

        //WTM:  Note:  This is needed because I needed to implement ITES5ValueCodeChunk which requires this.
        public ITES5Type TES5Type => throw new NotImplementedException();
    }
}