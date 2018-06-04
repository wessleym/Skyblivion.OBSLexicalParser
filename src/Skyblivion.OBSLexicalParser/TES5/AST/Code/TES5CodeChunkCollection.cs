using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5CodeChunkCollection : IEnumerable<ITES5CodeChunk>, ITES5ValueCodeChunk//ITES5ValueCodeChunk is needed for SetLevelFactory.
    {
        List<ITES5CodeChunk> values = new List<ITES5CodeChunk>();
        public void Add(ITES5CodeChunk value)
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

        public IEnumerable<string> Output => this.values.SelectMany(v => v.Output);

        public ITES5Type TES5Type => throw new NotImplementedException();//Required for ITES5ValueCodeChunk
    }
}