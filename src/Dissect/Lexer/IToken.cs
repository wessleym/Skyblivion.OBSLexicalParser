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
        string getType();
        /*
        * Returns the token value.
        */
        string getValue();
        /*
        * Returns the line on which the token was found.
        */
        int getLine();
    }
}