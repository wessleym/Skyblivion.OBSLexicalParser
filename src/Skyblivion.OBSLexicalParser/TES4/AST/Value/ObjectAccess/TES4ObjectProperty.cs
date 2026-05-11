using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System.Collections;
using System.Collections.Generic;

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

        public object Constant => StringValue;

        public string StringValue => this.parentReference.StringValue + "." + this.accessField.StringValue;

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            yield return this;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
