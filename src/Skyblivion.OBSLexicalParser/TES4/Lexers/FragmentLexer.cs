namespace Skyblivion.OBSLexicalParser.TES4.Lexers
{
    class FragmentLexer : OBScriptLexer
    {
        public FragmentLexer()
        {
            this.BuildObscriptLexer();
            this.Start("BlockScope");
        }
    }
}