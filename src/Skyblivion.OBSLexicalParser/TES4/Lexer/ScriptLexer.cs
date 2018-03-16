using Dissect.Lexer;

namespace Skyblivion.OBSLexicalParser.TES4.Lexer
{
    class ScriptLexer : OBScriptLexer
    {
        public ScriptLexer()
        {
            this.buildObscriptLexer();
            this.start("globalScope");
        }
    }
}