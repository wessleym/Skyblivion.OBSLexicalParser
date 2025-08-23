using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5Bool : ITES5Primitive
    {
        public bool Value { get; }
        public bool AllowSimplification { get; }
        public TES5Bool(bool b, bool allowSimplification = true)
        {
            this.Value = b;
            AllowSimplification = allowSimplification;
        }

        public string OutputSingle => Value ? "True" : "False";

        public IEnumerable<string> Output => [OutputSingle];

        public ITES5Type TES5Type => TES5BasicType.T_BOOL;
    }
}