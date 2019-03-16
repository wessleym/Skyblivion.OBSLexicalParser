using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Globalization;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5Float : TES5IntegerOrFloat
    {
        private readonly float f;
        public TES5Float(float f)
        {
            this.f = f;
        }

        public override IEnumerable<string> Output => new string[] { f.ToString(CultureInfo.InvariantCulture) };

        public override ITES5Type TES5Type => TES5BasicType.T_FLOAT;

        public override int ConvertedIntValue => (int)f;
    }
}