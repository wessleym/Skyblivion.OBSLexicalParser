using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5String : ITES5Primitive
    {
        private string str;
        public TES5String(string str)
        {
            if (!str.StartsWith("\"")) { str = "\"" + str; }
            if (!str.EndsWith("\"")) { str += "\""; }
            this.str = str;
        }

        public List<string> output()
        {
            return new List<string>() { this.str };
        }

        public ITES5Type getType()
        {
            return TES5BasicType.T_STRING;
        }

        public object getValue()
        {
            return this.str;
        }
    }
}
