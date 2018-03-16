namespace Dissect.Lexer.Recognizer
{
    /*
     * Recognizers are used by the lexer to process
     * the input string.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    public interface IRecognizer
    {
        /*
        * Returns a boolean value specifying whether
         * the string matches or not and if it does,
         * returns the match in the second variable.
        */
        bool match(string str, out string result);
    }
}