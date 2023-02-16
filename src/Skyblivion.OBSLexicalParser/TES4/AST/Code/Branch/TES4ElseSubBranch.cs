namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4ElseSubBranch : ITES4CodeChunk
    {
        public TES4CodeChunks CodeChunks { get; }
        public TES4ElseSubBranch(TES4CodeChunks codeChunks)
        {
            CodeChunks = codeChunks;
        }
    }
}