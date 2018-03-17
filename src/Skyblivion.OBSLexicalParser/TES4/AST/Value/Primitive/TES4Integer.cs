using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive
{
    class TES4Integer : ITES4Primitive, ITES4StringValue
    {
        private int data;
        public TES4Integer(int data)
        {
            this.data = data;
        }

        public object getData()
        {
            return this.data;
        }

        public string StringValue => data.ToString();

        public TES4Type getType()
        {
            return TES4Type.T_INT;
        }

        public bool hasFixedValue()
        {
            return true;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}
