using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.TES4.AST
{
    class TES4FragmentTarget
    {
        private TES4CodeChunks codeChunks;
        private string outputPath;
        /*
        * TES4FragmentTarget constructor.
        */
        public TES4FragmentTarget(TES4CodeChunks codeChunks, string outputPath)
        {
            this.codeChunks = codeChunks;
            this.outputPath = outputPath;
        }

        public TES4CodeChunks getCodeChunks()
        {
            return this.codeChunks;
        }

        public string getOutputPath()
        {
            return this.outputPath;
        }
    }
}