using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5Return : ITES5CodeChunk
    {
        private readonly ITES5Value? returnValue;
        public TES5Return(ITES5Value? returnValue = null)
        {
            this.returnValue = returnValue;
        }

        public IEnumerable<string> Output => new string[] { "Return" + (returnValue != null ? " " + returnValue.Output.Single() : "") };
    }
}