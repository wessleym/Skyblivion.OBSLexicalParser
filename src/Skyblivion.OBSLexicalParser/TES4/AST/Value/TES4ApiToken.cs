using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value
{
    class TES4ApiToken : ITES4Reference
    {
        private string token;
        public TES4ApiToken(string token)
        {
            this.token = token;
        }

        public object getData()
        {
            return StringValue;
        }

        public string StringValue => token;

        public bool hasFixedValue()
        {
            return true;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            if (predicate(this))
            {
                return new ITES4CodeFilterable[] { this };
            }
            return new ITES4CodeFilterable[] { };
        }
    }
}