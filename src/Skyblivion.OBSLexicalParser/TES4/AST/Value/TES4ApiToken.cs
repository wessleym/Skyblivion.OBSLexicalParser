using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value
{
    class TES4ApiToken : ITES4Reference
    {
        private readonly string token;
        public TES4ApiToken(string token)
        {
            this.token = token;
        }

        public object Data => StringValue;

        public string StringValue => token;

        public bool HasFixedValue => true;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            if (predicate(this))
            {
                return new ITES4CodeFilterable[] { this };
            }
            return new ITES4CodeFilterable[] { };
        }
    }
}