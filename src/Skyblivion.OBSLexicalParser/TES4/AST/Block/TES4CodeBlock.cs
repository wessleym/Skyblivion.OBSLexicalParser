using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4CodeBlock : ITES4CodeFilterable
    {
        public string BlockType { get; }
        public TES4BlockParameterList? BlockParameterList { get; }
        public TES4CodeChunks? Chunks { get; }
        public TES4CodeBlock(string blockType, TES4BlockParameterList? blockParameterList = null, TES4CodeChunks? chunks = null)
        {
            this.BlockType = blockType;
            this.BlockParameterList = blockParameterList;
            this.Chunks = chunks;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = new ITES4CodeFilterable[] { };
            if (this.BlockParameterList != null)
            {
                filtered = filtered.Concat(this.BlockParameterList.Filter(predicate));
            }

            if (this.Chunks != null)
            {
                filtered = filtered.Concat(this.Chunks.Filter(predicate));
            }

            return filtered.ToArray();
        }
    }
}