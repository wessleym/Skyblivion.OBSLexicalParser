using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4CodeBlock : ITES4CodeFilterable
    {
        private string blockType;
        private TES4CodeChunks chunks;
        private TES4BlockParameterList blockParameterList;
        public TES4CodeBlock(string blockType, TES4BlockParameterList blockParameterList = null, TES4CodeChunks chunks = null)
        {
            this.blockType = blockType;
            this.blockParameterList = blockParameterList;
            this.chunks = chunks;
        }

        public TES4BlockParameterList getBlockParameterList()
        {
            return this.blockParameterList;
        }

        public string getBlockType()
        {
            return this.blockType;
        }

        public TES4CodeChunks getChunks()
        {
            return this.chunks;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = new ITES4CodeFilterable[] { };
            if (this.blockParameterList != null)
            {
                filtered = filtered.Concat(this.blockParameterList.Filter(predicate));
            }

            if (this.chunks != null)
            {
                filtered = filtered.Concat(this.chunks.Filter(predicate));
            }

            return filtered.ToArray();
        }
    }
}