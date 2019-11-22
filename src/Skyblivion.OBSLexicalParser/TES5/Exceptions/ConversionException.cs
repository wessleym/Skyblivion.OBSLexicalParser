using System;

namespace Skyblivion.OBSLexicalParser.TES5.Exceptions
{
    class ConversionException : Exception
    {
        public bool Expected { get; private set; }//If true, exception will be logged.  If false, exception will be thrown.
        public ConversionException(string message, Exception? innerException = null, bool expected = false)
            : base(message, innerException)
        {
            Expected = expected;
        }
    }
}
