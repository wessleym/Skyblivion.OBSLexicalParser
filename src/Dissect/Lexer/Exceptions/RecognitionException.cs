using System;

namespace Dissect.Lexer.Exceptions
{
    /*
     * Thrown when a lexer is unable to extract another token.
     *
     * @author Jakub Lédl <jakubledl@gmail.com>
     */
    class RecognitionException : Exception
    {
        /*
        * Returns the source line number where the exception occured.
        */
        public int SourceLine { get; protected set; }
        /*
        * Constructor.
        */
        public RecognitionException(int line)
            : base("Cannot extract another token at line "+ line+".")
        {
            this.SourceLine = line;
        }
    }
}