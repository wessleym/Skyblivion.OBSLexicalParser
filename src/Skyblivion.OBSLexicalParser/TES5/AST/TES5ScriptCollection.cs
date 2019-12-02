using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5ScriptCollection
    {
        private readonly List<TES5Target> collection = new List<TES5Target>();
        public void Add(TES5Script script, string outputPath)
        {
            this.collection.Add(new TES5Target(script, outputPath));
        }
    }
}