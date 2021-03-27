using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive
{
    class TES4Integer : ITES4Primitive
    {
        public int IntValue { get; }
        public TES4Integer(int data)
        {
            this.IntValue = data;
        }

        public object Data => this.IntValue;

        public string StringValue => IntValue.ToString();

        public TES4Type Type => TES4Type.T_INT;

        public bool HasFixedValue => true;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}
