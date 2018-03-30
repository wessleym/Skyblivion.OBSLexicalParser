using Dissect.Lexer;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES4.Lexers
{
    class ArithLexer : SimpleLexer
    {
        public ArithLexer()
        {
            this.regex("INT", new Regex(@"^[1-9][0-9]*"));
            this.token("(");
            this.token(")");
            this.token("+");
            this.token("**");
            this.token("*");
            this.regex("WSP", new Regex(@"^[ \r\n\t]+"));
            this.skip("WSP");
        }
    }
}