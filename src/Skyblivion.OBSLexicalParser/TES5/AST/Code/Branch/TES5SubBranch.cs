using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch
{
    class TES5SubBranch
    {
        private ITES5Value expression;
        private TES5CodeScope codeScope;
        public TES5SubBranch(ITES5Value expression, TES5CodeScope codeScope)
        {
            this.expression = expression;
            this.codeScope = codeScope;
        }

        public TES5CodeScope getCodeScope()
        {
            return this.codeScope;
        }

        public void setCodeScope(TES5CodeScope codeScope)
        {
            this.codeScope = codeScope;
        }

        public void setExpression(ITES5Value expression)
        {
            this.expression = expression;
        }

        private string GetConditionLineOutput(string ifOrElseIf)
        {
            return ifOrElseIf + "(" + this.expression.output().Single() + ")";
        }

        public IEnumerable<string> GetOutput(string ifOrElseIf)
        {
            return (new string[] { GetConditionLineOutput(ifOrElseIf) })
                .Concat(this.getCodeScope().output());
        }
    }
}