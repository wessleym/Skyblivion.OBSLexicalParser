using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive
{
    class TES4String : ITES4Primitive
    {
        private string data;
        public TES4String(string data)
        {
            this.data = data.Trim('"');
        }

        public object getData()
        {
            return this.data;
        }

        public TES4Type getType()
        {
            return TES4Type.T_STRING;
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
