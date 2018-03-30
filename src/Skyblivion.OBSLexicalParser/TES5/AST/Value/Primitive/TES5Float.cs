using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5Float : ITES5Primitive
    {
        private float f;
        public TES5Float(float f)
        {
            this.f = f;
        }

        public IEnumerable<string> output()
        {
            return new string[] { f.ToString() };
        }

        public ITES5Type getType()
        {
            return TES5BasicType.T_FLOAT;
        }

        public object getValue()
        {
            return this.f;
        }
    }
}