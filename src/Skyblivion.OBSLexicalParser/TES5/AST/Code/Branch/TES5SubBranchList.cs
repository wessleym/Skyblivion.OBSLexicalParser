using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5SubBranchList
    {
        private List<TES5SubBranch> branchList = new List<TES5SubBranch>();
        public void add(TES5SubBranch declaration)
        {
            this.branchList.Add(declaration);
        }

        public IEnumerable<string> GetElseIfOutput()
        {
            return this.branchList.SelectMany(branch => branch.GetOutput("ElseIf"));
        }
    }
}