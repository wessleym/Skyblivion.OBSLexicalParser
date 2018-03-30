using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5Integer : ITES5Primitive
    {
        private int integer;
        public TES5Integer(int integer)
        {
            this.integer = integer;
        }

        public IEnumerable<string> output()
        {
            return new string[] { this.integer.ToString() };
        }

        public ITES5Type getType()
        {
            return TES5BasicType.T_INT;
        }

        public object getValue()
        {
            return this.integer;
        }
    }
}
