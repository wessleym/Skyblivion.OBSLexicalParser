using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5Bool : ITES5Primitive
    {
        private bool b;
        public TES5Bool(bool b)
        {
            this.b = b;
        }

        public IEnumerable<string> Output => new string[] { b ? "True" : "False" };

        public ITES5Type TES5Type => TES5BasicType.T_BOOL;
    }
}