using System;

namespace Skyblivion.OBSLexicalParser.TES5.Exceptions
{
    class ConversionException : Exception
    {
        public ConversionException(string message)
            : base(message)
        { }
        public ConversionException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
