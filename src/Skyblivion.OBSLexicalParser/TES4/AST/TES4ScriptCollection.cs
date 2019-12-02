using System.Collections;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST
{
    class TES4ScriptCollection : IEnumerable<TES4Target>
    {
        private readonly List<TES4Target> collection = new List<TES4Target>();
        public void Add(TES4Script script, string outputPath)
        {
            this.collection.Add(new TES4Target(script, outputPath));
        }

        public IEnumerator<TES4Target> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}