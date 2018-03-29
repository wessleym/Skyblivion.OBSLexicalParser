using Dissect.Lexer.Recognizer;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dissect.Lexer
{
    /*
     * SimpleLexer uses specified recognizers
     * without keeping track of state.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public class SimpleLexer : AbstractLexer
    {
        protected string[] skipTokens = new string[] { };
        protected Dictionary<string, IRecognizer> recognizers = new Dictionary<string, IRecognizer>();
        /*
        * Adds a new token definition. If given only one argument,
         * it assumes that the token type and recognized value are
         * identical.
        */
        public SimpleLexer token(string type, string value = null)
        {
            this.recognizers.Add(type, new SimpleRecognizer(value != null ? value : type));
            return this;
        }

        /*
        * Adds a new regex token definition.
        */
        public SimpleLexer regex(string type, Regex regex)
        {
            this.recognizers.Add(type, new RegexRecognizer(regex));
            return this;
        }

        /*
        * Marks the token types given as arguments to be skipped.
        */
        public SimpleLexer skip(params string[] tokens)
        {
            this.skipTokens = tokens;
            return this;
        }
        
        protected override bool shouldSkipToken(IToken token)
        {
            return skipTokens.Contains(token.getType());
        }
        
        protected override IToken extractToken(string str)
        {
            string value = null;
            string type = null;
            foreach (var kvp in recognizers)
            {
                IRecognizer recognizer = kvp.Value;
                string v;
                if (recognizer.match(str, out v))
                {
                    if (value == null || Util.Util.stringLength(v) > Util.Util.stringLength(value))
                    {
                        value = v;
                        type = kvp.Key;
                    }
                }
            }

            if (type != null)
            {
                return new CommonToken(type, value, this.getCurrentLine());
            }

            return null;
        }
    }
}