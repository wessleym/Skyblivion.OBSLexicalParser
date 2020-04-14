namespace Skyblivion.OBSLexicalParser.TES5.Exceptions
{
    class ConversionTypeMismatchException : ConversionException
    {
        public ConversionTypeMismatchException(string message, bool expected = false)
            : base(message, expected: expected)
        { }
    }
}
