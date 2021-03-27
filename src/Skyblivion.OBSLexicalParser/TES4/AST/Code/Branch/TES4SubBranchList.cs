using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4SubBranchList
    {
        public List<TES4SubBranch> Branches { get; } = new List<TES4SubBranch>();
        public void Add(TES4SubBranch declaration)
        {
            this.Branches.Add(declaration);
        }
    }
}