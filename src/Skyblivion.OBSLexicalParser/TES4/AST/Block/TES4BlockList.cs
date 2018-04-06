using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4BlockList
    {
        private List<TES4CodeBlock> blocks = new List<TES4CodeBlock>();
        public List<TES4CodeBlock> getBlocks()
        {
            return this.blocks;
        }

        public void add(TES4CodeBlock block)
        {
            this.blocks.Add(block);
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return blocks.SelectMany(b=>b.Filter(predicate)).ToArray();
        }
    }
}