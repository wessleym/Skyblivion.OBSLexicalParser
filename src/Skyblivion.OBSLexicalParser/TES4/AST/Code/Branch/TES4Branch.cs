using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4Branch : ITES4Value, ITES4CodeChunk
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
    }
}
