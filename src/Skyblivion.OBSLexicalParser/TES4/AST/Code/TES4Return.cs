using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4Return : ITES4CodeChunk, ITES4CodeChunkContainer
    {
        public TES4Return() { }

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
