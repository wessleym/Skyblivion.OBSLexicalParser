using Dissect.Lexer;

namespace Skyblivion.OBSLexicalParser.TES4.Lexers
{
    class ArithLexer : SimpleLexer
    {
        public ArithLexer()
        {
            this.Regex("INT", @"^[1-9][0-9]*");
            this.Token("(");
            this.Token(")");
            this.Token("+");
            this.Token("**");
            this.Token("*");
            this.Regex("WSP", @"^[ \r\n\t]+");
            this.Skip("WSP");
        }
    }
}