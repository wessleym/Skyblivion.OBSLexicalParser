using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value
{
    class TES4ApiToken : ITES4Reference
    {
        private readonly string token;
        public TES4ApiToken(string token)
        {
            this.token = token;
        }

        public object Constant => StringValue;

        public string StringValue => token;

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