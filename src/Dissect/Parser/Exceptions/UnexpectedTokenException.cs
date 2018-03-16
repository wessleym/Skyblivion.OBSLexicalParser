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
        const string MESSAGE = "Unexpected %s at line %d.  Expected one of %s.";
        protected IToken token;
        protected string[] expected;
        /*
        * Constructor.
        */
        public UnexpectedTokenException(IToken token, string[] expected)
            : base(string.Format(MESSAGE, token.getValue() == token.getType() ? token.getValue() + " (" + token.getType() + ")" : token.getType(), token.getLine(), string.Join(", ", expected)))
        {
            this.token = token;
            this.expected = expected;
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