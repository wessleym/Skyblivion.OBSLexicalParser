namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5Target
    {
        private TES5Script script;
        private string outputPath;
        public TES5Target(TES5Script script, string outputPath)
        {
            this.script = script;
            this.outputPath = outputPath;
        }

        public TES5Script getScript()
        {
            return this.script;
        }

        public string getOutputPath()
        {
            return this.outputPath;
        }
    }
}