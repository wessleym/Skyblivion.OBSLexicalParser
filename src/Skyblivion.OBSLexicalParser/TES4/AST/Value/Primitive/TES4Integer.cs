using Skyblivion.OBSLexicalParser.TES4.Types;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive
{
    class TES4Integer : ITES4Primitive
    {
        public int IntValue { get; }
        public TES4Integer(int data)
        {
            this.IntValue = data;
        }

        public object Constant => this.IntValue;

        public string StringValue => IntValue.ToString();

        public TES4Type Type => TES4Type.T_INT;
    }
}
