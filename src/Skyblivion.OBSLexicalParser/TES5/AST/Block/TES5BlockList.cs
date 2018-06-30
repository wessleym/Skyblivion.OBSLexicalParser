using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    class TES5BlockList : ITES5Outputtable, IEnumerable<ITES5CodeBlock>
    {
        public List<ITES5CodeBlock> Blocks { get; private set; } = new List<ITES5CodeBlock>();

        public IEnumerable<string> Output => Blocks.SelectMany(b => b.Output);

        public void Add(ITES5CodeBlock block)
        {
            Blocks.Add(block);
        }

        public IEnumerator<ITES5CodeBlock> GetEnumerator()
        {
            return Blocks.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}