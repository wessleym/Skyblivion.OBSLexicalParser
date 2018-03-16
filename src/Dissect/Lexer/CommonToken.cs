namespace Dissect.Lexer
{
    /*
     * A simple token representation.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public class CommonToken : IToken
    {
        protected string type;
        protected string value;
        protected int line;
        /*
        * Constructor.
        */
        public CommonToken(string type, string value, int line)
        {
            this.type = type;
            this.value = value;
            this.line = line;
        }

        public string getType()
        {
            return this.type;
        }

        public string getValue()
        {
            return this.value;
        }

        public int getLine()
        {
            return this.line;
        }
    }
}