using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5ElseSubBranch
    {
        private readonly TES5CodeScope codeScope;
        public TES5ElseSubBranch(TES5CodeScope codeScope)
        {
            this.codeScope = codeScope;
        }

        public IEnumerable<string> Output()
        {
            return (new string[] { "Else" })
                .Concat(this.codeScope.Output.Select(o => TES5Script.Indent + o));
        }
    }
}