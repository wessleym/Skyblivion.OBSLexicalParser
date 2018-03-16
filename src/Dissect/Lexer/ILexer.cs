using Dissect.Lexer.TokenStream;

namespace Dissect.Lexer
{
    /*
     * A lexer takes an input string and processes
     * it into a token stream.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    interface ILexer
    {
        /*
        * Lexes the given string, returning a token stream.
        */
        ArrayTokenStream lex(string str);
    }
}