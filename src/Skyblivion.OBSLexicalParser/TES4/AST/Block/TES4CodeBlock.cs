using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4CodeBlock : ITES4CodeBlockOrComment
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
    }
}