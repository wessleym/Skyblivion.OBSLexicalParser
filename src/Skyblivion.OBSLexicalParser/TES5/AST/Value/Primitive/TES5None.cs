using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5None : ITES5Primitive
    {
        public static readonly TES5BasicType TES5TypeStatic = TES5BasicType.T_FORM;
        public IEnumerable<string> Output => new string[] { "None" };

        public ITES5Type TES5Type => TES5TypeStatic;
    }
}