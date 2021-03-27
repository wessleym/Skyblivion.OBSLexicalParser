namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    class TES5Target
    {
        public TES5Script Script { get; }
        public string OutputPath { get; }
        public TES5Target(TES5Script script, string outputPath)
        {
            this.Script = script;
            this.OutputPath = outputPath;
        }
    }
}