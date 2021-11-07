using Dissect.Lexer.Exceptions;
using Dissect.Lexer.TokenStream;
using Dissect.Parser;
using Dissect.Util;
using System.Collections.Generic;
using System.Linq;

namespace Dissect.Lexer
{
    /*
     * A base class for a lexer. A superclass simply
     * has to implement the extractToken and shouldSkipToken methods. Both
     * SimpleLexer and StatefulLexer extend this class.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public abstract class AbstractLexer : ILexer
    {
        /*
        * Returns the current line.
        */
        protected int Line { get; private set; } = 1;

        /*
        * Attempts to extract another token from the string.
         * Returns the token on success or null on failure.
        */
        protected abstract IToken? ExtractToken(string str);
        /*
        * Should given token be skipped?
        */
        protected abstract bool ShouldSkipToken(IToken token);
        protected virtual void ResetStatesForNewString() { }
        public ArrayTokenStream Lex(string str)
        {
            Line = 1;//WTM:  Fixed
            ResetStatesForNewString();
            // normalize line endings
            str = str.Replace("\r\n", "\n").Replace("\r", "\n");
            List<IToken> tokens = new List<IToken>();
            int position = 0;
            string originalString = str;
            int originalLength = Util.Util.StringLength(str);
            while (true)
            {
                IToken? token = this.ExtractToken(str);
                if (token == null)
                {
                    break;
                }

                if (!this.ShouldSkipToken(token))
                {
                    tokens.Add(token);
                }

                int shift = Util.Util.StringLength(token.Value);
                position += shift;
                // update line + offset
                if (position > 0)
                {
                    this.Line = originalString.Substring(0, position).Count(c => c == '\n') + 1;
                }

                str = Util.Util.Substring(str, shift);
            }

            if (position != originalLength)
            {
                throw new RecognitionException(this.Line);
            }

            tokens.Add(new CommonToken(Parser.Parser.EOF_TOKEN_TYPE, "", this.Line));
            return new ArrayTokenStream(tokens);
        }
    }
}