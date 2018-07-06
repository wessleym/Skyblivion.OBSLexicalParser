namespace Skyblivion.OBSLexicalParser.TES4.Lexers
{
    class ScriptLexer : OBScriptLexer
    {
        public ScriptLexer()
        {
            this.BuildObscriptLexer();
            this.Start("globalScope");
        }
    }
}