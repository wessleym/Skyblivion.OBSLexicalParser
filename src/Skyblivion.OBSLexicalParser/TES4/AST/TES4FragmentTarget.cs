using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.TES4.AST
{
    class TES4FragmentTarget
    {
        public TES4CodeChunks CodeChunks { get; private set; }
        public string OutputPath { get; private set; }
        public TES4FragmentTarget(TES4CodeChunks codeChunks, string outputPath)
        {
            this.CodeChunks = codeChunks;
            this.OutputPath = outputPath;
        }
    }
}