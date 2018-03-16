namespace Skyblivion.OBSLexicalParser.TES4.AST
{
    class TES4Target
    {
        private TES4Script script;
        private string outputPath;
        public TES4Target(TES4Script script, string outputPath)
        {
            this.script = script;
            this.outputPath = outputPath;
        }

        public TES4Script getScript()
        {
            return this.script;
        }

        public string getOutputPath()
        {
            return this.outputPath;
        }
    }
}