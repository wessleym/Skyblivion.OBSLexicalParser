using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5ElseClause : ITES5Outputtable
    {
        private readonly TES5CodeScope codeScope;
        public TES5ElseClause(TES5CodeScope codeScope)
        {
            this.codeScope = codeScope;
        }

        public IEnumerable<string> Output => new string[] { "Else" }.Concat(this.codeScope.Output.SkipEmptyStartAndIndent());
    }
}