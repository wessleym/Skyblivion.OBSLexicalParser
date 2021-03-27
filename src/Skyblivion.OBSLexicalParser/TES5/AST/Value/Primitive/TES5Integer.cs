using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5Integer : TES5IntegerOrFloat
    {
        public int IntValue { get; }
        public TES5Integer(int integer)
        {
            this.IntValue = integer;
        }

        public override IEnumerable<string> Output => new string[] { this.IntValue.ToString() };

        public override ITES5Type TES5Type => TES5BasicType.T_INT;

        public override int ConvertedIntValue => IntValue;
    }
}
