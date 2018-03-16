using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4SubBranchList
    {
        private List<TES4SubBranch> variableList = new List<TES4SubBranch>();
        public void add(TES4SubBranch declaration)
        {
            this.variableList.Add(declaration);
        }

        public List<TES4SubBranch> getSubBranches()
        {
            return this.variableList;
        }
    }
}