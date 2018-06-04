using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4BlockList : ITES4CodeFilterable
    {
        public List<TES4CodeBlock> Blocks { get; private set; } = new List<TES4CodeBlock>();

        public void Add(TES4CodeBlock block)
        {
            this.Blocks.Add(block);
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.Blocks.SelectMany(b=>b.Filter(predicate)).ToArray();
        }
    }
}