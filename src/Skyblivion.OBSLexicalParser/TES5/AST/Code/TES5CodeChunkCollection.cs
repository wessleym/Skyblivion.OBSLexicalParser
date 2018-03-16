using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5CodeChunkCollection : IEnumerable<ITES5CodeChunk>
    {
        List<ITES5CodeChunk> values = new List<ITES5CodeChunk>();
        public void add(ITES5CodeChunk value)
        {
            values.Add(value);
        }

        public IEnumerator<ITES5CodeChunk> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}