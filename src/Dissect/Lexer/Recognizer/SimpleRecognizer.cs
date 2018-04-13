using System;

namespace Dissect.Lexer.Recognizer
{
    /*
     * SimpleRecognizer matches a string by a simple
     * strpos match.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class SimpleRecognizer : IRecognizer
    {
        protected string needle;
        protected bool ignoreCase;//WTM:  Change:  I added ignoreCase and its functionality.
        /*
        * Constructor.
        */
        public SimpleRecognizer(string needle, bool ignoreCase = false)
        {
            this.needle = needle;
            this.ignoreCase = ignoreCase;
        }

        private bool NeedleStartsWith(string haystack)
        {
            return
                !ignoreCase ?
                haystack.StartsWith(needle) :
                haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public bool match(string haystack, out string result)
        {
            if (NeedleStartsWith(haystack))
            {
                result = needle;
                return true;
            }
            result = null;
            return false;
        }
    }
}