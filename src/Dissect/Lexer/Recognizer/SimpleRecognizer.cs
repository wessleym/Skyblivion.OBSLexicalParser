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
        protected string str; /*
        * Constructor.
        */
        public SimpleRecognizer(string str)
        {
            this.str = str;
        }

        public bool match(string str, out string result)
        {
            if (str.StartsWith(this.str))
            {
                result = this.str;
                return true;
            }
            result = null;
            return false;
        }
    }
}