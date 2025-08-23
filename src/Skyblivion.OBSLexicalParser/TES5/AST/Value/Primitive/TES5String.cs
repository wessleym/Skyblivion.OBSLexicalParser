using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5String : ITES5Primitive
    {
        public string StringValue { get; }
        public TES5String(string str)
        {
            if (!str.StartsWith("\"")) { str = "\"" + str; }
            if (!str.EndsWith("\"")) { str += "\""; }
            this.StringValue = str;
        }

        public string OutputSingle => this.StringValue;

        public IEnumerable<string> Output => [OutputSingle];

        public ITES5Type TES5Type => TES5BasicType.T_STRING;
    }
}
