using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess
{
    class TES4ObjectProperty : ITES4ObjectAccess, ITES4Reference
    {
        private TES4ApiToken parentReference;
        private TES4ApiToken accessField;
        public TES4ObjectProperty(TES4ApiToken parentReference, TES4ApiToken accessField)
        {
            this.parentReference = parentReference;
            this.accessField = accessField;
        }

        public object getData()
        {
            return StringValue;
        }

        public string StringValue => this.parentReference.StringValue + "." + this.accessField.StringValue;

        public bool hasFixedValue()
        {
            return false;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}
