using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5SubBranch
    {
        private readonly ITES5Value expression;
        public TES5CodeScope CodeScope { get; private set; }
        public TES5SubBranch(ITES5Value expression, TES5CodeScope codeScope)
        {
            this.expression = expression;
            this.CodeScope = codeScope;
        }

        private string GetConditionLineOutput(string ifOrElseIf)
        {
            return ifOrElseIf + " (" + this.expression.Output.Single() + ")";
        }

        public IEnumerable<string> GetOutput(string ifOrElseIf)
        {
            return (new string[] { GetConditionLineOutput(ifOrElseIf) })
                .Concat(this.CodeScope.Output.Select(o => TES5Script.Indent + o));
        }
    }
}