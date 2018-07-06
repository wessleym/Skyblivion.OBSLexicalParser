using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess
{
    class TES4ObjectProperty : ITES4ObjectAccess, ITES4Reference
    {
        private readonly TES4ApiToken parentReference;
        private readonly TES4ApiToken accessField;
        public TES4ObjectProperty(TES4ApiToken parentReference, TES4ApiToken accessField)
        {
            this.parentReference = parentReference;
            this.accessField = accessField;
        }

        public object Data => StringValue;

        public string StringValue => this.parentReference.StringValue + "." + this.accessField.StringValue;

        public bool HasFixedValue => false;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}
