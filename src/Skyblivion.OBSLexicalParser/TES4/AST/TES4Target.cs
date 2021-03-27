namespace Skyblivion.OBSLexicalParser.TES4.AST
{
    class TES4Target
    {
        public TES4Script Script { get; }
        public string OutputPath { get; }
        public TES4Target(TES4Script script, string outputPath)
        {
            this.Script = script;
            this.OutputPath = outputPath;
        }
    }
}