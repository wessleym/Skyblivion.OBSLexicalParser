namespace Dissect.Lexer
{
    /*
     * A simple token representation.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public class CommonToken : IToken
    {
        public string Type { get; protected set; }
        public string Value { get; protected set; }
        public int Line { get; protected set; }
        /*
        * Constructor.
        */
        public CommonToken(string type, string value, int line)
        {
            this.Type = type;
            this.Value = value;
            this.Line = line;
        }
    }
}