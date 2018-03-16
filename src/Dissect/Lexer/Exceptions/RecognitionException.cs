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
        protected int sourceLine;
        /*
        * Constructor.
        */
        public RecognitionException(int line)
            : base("Cannot extract another token at line "+ line+".")
        {
            this.sourceLine = line;
        }

        /*
        * Returns the source line number where the exception occured.
        */
        public int getSourceLine()
        {
            return this.sourceLine;
        }
    }
}