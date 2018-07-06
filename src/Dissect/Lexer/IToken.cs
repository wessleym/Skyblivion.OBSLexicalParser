namespace Dissect.Lexer
{
    /*
     * A common contract for tokens.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public interface IToken
    {
        /*
        * Returns the token type.
        */
        string Type { get; }

        /*
        * Returns the token value.
        */
        string Value { get; }

        /*
        * Returns the line on which the token was found.
        */
        int Line { get; }
    }
}