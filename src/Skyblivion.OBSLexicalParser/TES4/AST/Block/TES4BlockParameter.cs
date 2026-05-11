using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4BlockParameter : ITES4CodeChunk
    {
        public string BlockParameter { get; }
        public TES4BlockParameter(string blockParameter)
        {
            this.BlockParameter = blockParameter;
        }
    }
}