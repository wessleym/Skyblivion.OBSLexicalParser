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
        protected IToken token;
        protected string[] expected;
        /*
        * Constructor.
        */
        public UnexpectedTokenException(IToken token, string[] expected)
            : base(GetMessage(token, expected))
        {
            this.token = token;
            this.expected = expected;
        }

        private static string GetMessage(IToken token, string[] expected)
        {
            return "Unexpected " + (token.getValue() != token.getType() ? token.getValue() + " (" + token.getType() + ")" : token.getType()) + @" at line " + token.getLine() + @".  Expected one of " + string.Join(", ", expected) + @".";
        }

        /*
        * Returns the unexpected token.
        */
        public IToken getToken()
        {
            return this.token;
        }

        /*
        * Returns the expected token types.
        */
        public string[] getExpected()
        {
            return this.expected;
        }
    }
}