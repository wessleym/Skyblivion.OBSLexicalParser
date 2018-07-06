using System;
using System.Collections;
using System.Collections.Generic;

namespace Dissect.Lexer.TokenStream
{
    /*
     * A simple array based implementation of a token stream.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public class ArrayTokenStream : ITokenStream, IEnumerable<IToken>
    {
        protected readonly IList<IToken> tokens;
        public int Position { get; protected set; } = 0;
        /*
        * Constructor.
        */
        public ArrayTokenStream(IList<IToken> tokens)
        {
            this.tokens = tokens;
        }

        public IToken CurrentToken => this.tokens[this.Position];

        public IToken LookAhead(int n)
        {
            int newPosition = this.Position + n;
            if (newPosition<tokens.Count)
            {
                return tokens[newPosition];
            }

            throw new ArgumentOutOfRangeException("Invalid look-ahead.");
        }

        public IToken Get(int n)
        {
            try
            {
                return this.tokens[n];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException("Invalid index.", ex);
            }
        }

        public void Move(int n)
        {
            if (n>tokens.Count - 1)
            {
                throw new ArgumentOutOfRangeException("Invalid index to move to.");
            }

            this.Position = n;
        }

        public void Seek(int n)
        {
            int newPosition = this.Position + n;
            if (newPosition>tokens.Count - 1)
            {
                throw new ArgumentOutOfRangeException("Invalid seek.");
            }

            this.Position = newPosition;
        }

        public void Next()
        {
            int newPosition = this.Position + 1;
            if (newPosition > tokens.Count - 1)
            {
                throw new ArgumentOutOfRangeException("Attempting to move beyond the end of the stream.");
            }

            this.Position= newPosition;
        }

        public int Count => tokens.Count;

        public IEnumerator<IToken> GetEnumerator()
        {
            return tokens.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}