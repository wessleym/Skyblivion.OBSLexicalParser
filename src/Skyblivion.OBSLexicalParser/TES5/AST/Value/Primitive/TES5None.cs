using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5None : ITES5Primitive
    {
        public List<string> output()
        {
            return new List<string>() { "None" };
        }

        public ITES5Type getType()
        {
            return TES5BasicType.T_FORM;
        }

        public object getValue()
        {
            return null;
        }
    }
}