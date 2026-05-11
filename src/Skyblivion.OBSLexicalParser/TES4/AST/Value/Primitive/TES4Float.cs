using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Types;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive
{
    class TES4Float : ITES4Primitive
    {
        private readonly float data;
        public TES4Float(float data)
        {
            this.data = data;
        }

        public object Constant => this.data;

        public string StringValue => this.data.ToString(CultureInfo.InvariantCulture);

        public TES4Type Type => TES4Type.T_FLOAT;

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