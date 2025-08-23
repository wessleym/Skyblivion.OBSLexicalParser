using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5Return : ITES5CodeChunk, ITES5OutputtableSingle
    {
        private readonly ITES5Value? returnValue;
        public TES5Return(ITES5Value? returnValue = null)
        {
            this.returnValue = returnValue;
        }

        public string OutputSingle => "Return" + (returnValue != null ? " " + returnValue.OutputSingle : "");

        public IEnumerable<string> Output => [OutputSingle];
    }
}