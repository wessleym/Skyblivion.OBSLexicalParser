using Dissect.Lexer;
using Dissect.Lexer.TokenStream;

namespace Dissect.Parser
{
    /*
     * The parser interface.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public abstract class Parser
    {
        /*
        * The token type that represents an EOF.
        */
        public const string EOF_TOKEN_TYPE = "eof";
        /*
        * Parses a token stream and returns the semantical value
         * of the input.
        */
        protected abstract IToken parse(ITokenStream stream);
    }
}