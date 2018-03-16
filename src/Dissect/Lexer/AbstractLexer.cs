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
        private int line = 1;
        /*
        * Returns the current line.
        */
        protected int getCurrentLine()
        {
            return this.line;
        }

        /*
        * Attempts to extract another token from the string.
         * Returns the token on success or null on failure.
        */
        protected abstract IToken extractToken(string str);
        /*
        * Should given token be skipped?
        */
        protected abstract bool shouldSkipToken(IToken token);
        public ArrayTokenStream lex(string str)
        {
            // normalize line endings
            str = str.Replace("\r\n", "\n").Replace("\r", "\n");
            List<IToken> tokens = new List<IToken>();
            int position = 0;
            string originalString = str;
            int originalLength = Dissect.Util.Util.stringLength(str);
            while (true)
            {
                IToken token = this.extractToken(str);
                if (token == null)
                {
                    break;
                }

                if (!this.shouldSkipToken(token))
                {
                    tokens.Add(token);
                }

                int shift = Dissect.Util.Util.stringLength(token.getValue());
                position += shift;
                // update line + offset
                if (position > 0)
                {
                    this.line = originalString.Substring(0, position).Cast<char>().Count(c => c == '\n') + 1;
                }

                str = Dissect.Util.Util.substring(str, shift);
            }

            if (position == originalLength)
            {
                throw new RecognitionException(this.line);
            }

            tokens.Add(new CommonToken(Parser.Parser.EOF_TOKEN_TYPE, "", this.line));
            return new ArrayTokenStream(tokens);
        }
    }
}