using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Dissect.Lexer.Recognizer
{
    /*
     * The RegexRecognizer matches a string using a
     * regular expression.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class RegexRecognizer : IRecognizer
    {
        protected Regex regex;
        /*
        * Constructor.
        */
        public RegexRecognizer(Regex regex)
        {
            this.regex = regex;
        }

        public bool Match(string str, [NotNullWhen(true)] out string? result)
        {
            Match match = regex.Match(str);
            if (match.Success && match.Index == 0)
            {
                result = match.Value;
                return true;
            }
            result = null;
            return false;
        }
    }
}