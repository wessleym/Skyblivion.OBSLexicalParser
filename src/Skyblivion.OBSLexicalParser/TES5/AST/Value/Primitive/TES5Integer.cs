using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    class TES5Integer : TES5IntegerOrFloat
    {
        public int IntValue { get; }
        public TES5Integer(int integer)
        {
            this.IntValue = integer;
        }

        public override string OutputSingle => this.IntValue.ToString();

        public override ITES5Type TES5Type => TES5BasicType.T_INT;

        public override int ConvertedIntValue => IntValue;
    }
}
