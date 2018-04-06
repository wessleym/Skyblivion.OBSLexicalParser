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
        protected IList<IToken> tokens;
        protected int position = 0;
        /*
        * Constructor.
        */
        public ArrayTokenStream(IList<IToken> tokens)
        {
            this.tokens = tokens;
        }

        public int getPosition()
        {
            return this.position;
        }

        public IToken getCurrentToken()
        {
            return this.tokens[this.position];
        }

        public IToken lookAhead(int n)
        {
            int newPosition = this.position + n;
            if (newPosition<tokens.Count)
            {
                return tokens[newPosition];
            }

            throw new ArgumentOutOfRangeException("Invalid look-ahead.");
        }

        public IToken get(int n)
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

        public void move(int n)
        {
            if (n>tokens.Count - 1)
            {
                throw new ArgumentOutOfRangeException("Invalid index to move to.");
            }

            this.position = n;
        }

        public void seek(int n)
        {
            int newPosition = this.position + n;
            if (newPosition>tokens.Count - 1)
            {
                throw new ArgumentOutOfRangeException("Invalid seek.");
            }

            this.position = newPosition;
        }

        public void next()
        {
            int newPosition = this.position + 1;
            if (newPosition > tokens.Count - 1)
            {
                throw new ArgumentOutOfRangeException("Attempting to move beyond the end of the stream.");
            }

            this.position= newPosition;
        }
        
        public int count()
        {
            return tokens.Count;
        }

        public IEnumerator<IToken> GetEnumerator()
        {
            return ((IEnumerable<IToken>)tokens).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}