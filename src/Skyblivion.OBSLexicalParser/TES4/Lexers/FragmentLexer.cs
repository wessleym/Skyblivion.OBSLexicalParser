using Dissect.Lexer;

namespace Skyblivion.OBSLexicalParser.TES4.Lexers
{
    class FragmentLexer : OBScriptLexer
    {
        public FragmentLexer()
        {
            this.buildObscriptLexer();
            this.start("BlockScope");
        }
    }
}