using System.Collections.Generic;

namespace Dissect.Lexer.TokenStream
{
    /*
     * A common contract for all token stream classes.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public interface ITokenStream : IEnumerable<IToken>
    {
        /*
        * Returns the current position in the stream.
        */
        int getPosition();
        /*
        * Retrieves the current token.
        */
        IToken getCurrentToken();
        /*
        * Returns a look-ahead token. Negative values are allowed
        * and serve as look-behind.
        */
        IToken lookAhead(int n);
        /*
        * Returns the token at absolute position n.
        */
        IToken get(int n);
        /*
        * Moves the cursor to the absolute position n.
        */
        void move(int n);
        /*
        * Moves the cursor by n, relative to the current position.
        */
        void seek(int n);
        /*
        * Moves the cursor to the next token.
        */
        void next();
    }
}