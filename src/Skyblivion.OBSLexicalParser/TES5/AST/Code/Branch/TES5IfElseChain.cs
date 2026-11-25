using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5IfElseChain : ITES5CodeChunk
    {
        public TES5IfElseClause IfClause { get; }
        public List<TES5IfElseClause>? ElseIfClauses { get; }
        public TES5ElseClause? ElseClause { get; }
        public TES5IfElseChain(TES5IfElseClause ifClause, List<TES5IfElseClause>? elseIfClauses, TES5ElseClause? elseClause)
        {
            this.IfClause = ifClause;
            this.ElseIfClauses = elseIfClauses;
            this.ElseClause = elseClause;
        }

        public IEnumerable<string> Output
        {
            get
            {
                IEnumerable<string> lines = new string[] { "" }.Concat(this.IfClause.GetOutput("If"));
                if (this.ElseIfClauses != null)
                {
                    lines = lines.Concat(this.ElseIfClauses.SelectMany(branch => new string[] { "" }.Concat(branch.GetOutput("ElseIf"))));
                }
                if (this.ElseClause != null)
                {
                    lines = lines.Concat(new string[] { "" }).Concat(this.ElseClause.Output);
                }
                lines = lines.Concat(new string[] { "EndIf" });
                return lines;
            }
        }
    }
}