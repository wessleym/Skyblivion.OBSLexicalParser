using Dissect.Lexer;

namespace Skyblivion.OBSLexicalParser.TES4.Lexers
{
    class ArithLexer : SimpleLexer
    {
        public ArithLexer()
        {
            this.regex("INT", @"^[1-9][0-9]*");
            this.token("(");
            this.token(")");
            this.token("+");
            this.token("**");
            this.token("*");
            this.regex("WSP", @"^[ \r\n\t]+");
            this.skip("WSP");
        }
    }
}