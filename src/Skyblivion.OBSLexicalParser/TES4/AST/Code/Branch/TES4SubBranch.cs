using Skyblivion.OBSLexicalParser.TES4.AST.Value;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4SubBranch
    {
        public ITES4Value Expression { get; }
        public TES4CodeChunks CodeChunks { get; }
        public TES4SubBranch(ITES4Value expression, TES4CodeChunks codeChunks)
        {
            this.Expression = expression;
            this.CodeChunks = codeChunks;
        }
    }
}