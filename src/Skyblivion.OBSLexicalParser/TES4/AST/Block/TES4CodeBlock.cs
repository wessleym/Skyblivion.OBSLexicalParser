using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4CodeBlock : ITES4CodeBlockOrComment, ITES4CodeChunkContainer
    {
        public string BlockType { get; }
        public List<TES4BlockParameter> BlockParameterList { get; }
        public TES4CodeChunks Chunks { get; }
        public TES4CodeBlock(string blockType, List<TES4BlockParameter> blockParameterList, TES4CodeChunks chunks)
        {
            this.BlockType = blockType;
            this.BlockParameterList = blockParameterList;
            this.Chunks = chunks;
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            return this.BlockParameterList.Concat(this.Chunks).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}