using Dissect.Lexer.Recognizer;
using System;
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
        protected string[] SkipTokens = new string[] { };
        protected readonly Dictionary<string, IRecognizer> Recognizers = new Dictionary<string, IRecognizer>();
        /*
        * Adds a new token definition. If given only one argument,
         * it assumes that the token type and recognized value are
         * identical.
        */
        public SimpleLexer Token(string type, string? value = null)
        {
            if (value == null) { value = type; }
            this.Recognizers.Add(type, new SimpleRecognizer(value));
            return this;
        }

        /*
        * Adds a new regex token definition.
        */
        public SimpleLexer Regex(string type, Regex regex)
        {
            if (!regex.Options.HasFlag(RegexOptions.Compiled))
            {
                throw new InvalidOperationException("Regex was not compiled.");
            }
            this.Recognizers.Add(type, new RegexRecognizer(regex));
            return this;
        }
        private SimpleLexer Regex(string type, string pattern, RegexOptions options)
        {
            Regex(type, new Regex(pattern, options));
            return this;
        }
        public SimpleLexer Regex(string type, string pattern)
        {
            Regex(type, pattern, RegexOptions.Compiled);
            return this;
        }
        public SimpleLexer RegexIgnoreCase(string type, string pattern)
        {
            Regex(type, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return this;
        }

        /*
        * Marks the token types given as arguments to be skipped.
        */
        public SimpleLexer Skip(params string[] tokens)
        {
            this.SkipTokens = tokens;
            return this;
        }
        
        protected override bool ShouldSkipToken(IToken token)
        {
            return SkipTokens.Contains(token.Type);
        }
        
        protected override IToken? ExtractToken(string str)
        {
            string? value = null;
            string? type = null;
            foreach (var kvp in Recognizers)
            {
                IRecognizer recognizer = kvp.Value;
                string? v;
                if (recognizer.Match(str, out v))
                {
                    if (value == null || Util.Util.StringLength(v) > Util.Util.StringLength(value))
                    {
                        value = v;
                        type = kvp.Key;
                    }
                }
            }

            if (type != null)
            {
                return new CommonToken(type, value!, this.Line);
            }

            return null;
        }
    }
}