using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Globalization;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5Float : TES5IntegerOrFloat
    {
        public float Value { get; }
        public TES5Float(float value)
        {
            this.Value = value;
        }

        public override string OutputSingle => Value.ToString(CultureInfo.InvariantCulture);

        public override ITES5Type TES5Type => TES5BasicType.T_FLOAT;

        public override int ConvertedIntValue => (int)Value;
    }
}