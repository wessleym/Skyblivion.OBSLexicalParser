using Dissect.Lexer;
using System;

namespace Dissect.Parser.Exceptions
{
    /*
     * Thrown when a parser encounters an unexpected token.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public class UnexpectedTokenException : Exception
    {
        /*
        * Returns the unexpected token.
        */
        public IToken Token { get; protected set; }
        /*
        * Returns the expected token types.
        */
        public string[] Expected { get; protected set; }
        /*
        * Constructor.
        */
        public UnexpectedTokenException(IToken token, string[] expected)
            : base(GetMessage(token, expected))
        {
            this.Token = token;
            this.Expected = expected;
        }

        private static string GetMessage(IToken token, string[] expected)
        {
            return "Unexpected " + (token.Value!= token.Type? token.Value+ " (" + token.Type+ ")" : token.Type) + @" at line " + token.Line+ @".  Expected one of " + string.Join(", ", expected) + @".";
        }
    }
}