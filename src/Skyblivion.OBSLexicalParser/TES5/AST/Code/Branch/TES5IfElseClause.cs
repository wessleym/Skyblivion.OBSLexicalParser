using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5IfElseClause
    {
        private readonly ITES5Value condition;
        public TES5CodeScope CodeScope { get; }
        public TES5IfElseClause(ITES5Value condition, TES5CodeScope codeScope)
        {
            this.condition = condition;
            this.CodeScope = codeScope;
        }

        private string GetConditionLineOutput(string ifOrElseIf)
        {
            return ifOrElseIf + " " + this.condition.OutputSingle;
        }

        public IEnumerable<string> GetOutput(string ifOrElseIf)
        {
            return (new string[] { GetConditionLineOutput(ifOrElseIf) })
                .Concat(this.CodeScope.Output.SkipEmptyStartAndIndent());
        }
    }
}