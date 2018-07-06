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
        int Position { get; }

        /*
        * Retrieves the current token.
        */
        IToken CurrentToken { get; }

        /*
        * Returns a look-ahead token. Negative values are allowed
        * and serve as look-behind.
        */
        IToken LookAhead(int n);
        /*
        * Returns the token at absolute position n.
        */
        IToken Get(int n);
        /*
        * Moves the cursor to the absolute position n.
        */
        void Move(int n);
        /*
        * Moves the cursor by n, relative to the current position.
        */
        void Seek(int n);
        /*
        * Moves the cursor to the next token.
        */
        void Next();
    }
}