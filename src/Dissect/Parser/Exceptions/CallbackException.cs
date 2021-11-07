using Dissect.Lexer;
using System;

namespace Dissect.Parser.Exceptions
{
    //WTM:  Added
    public class CallbackException : Exception
    {
        public CallbackException(IToken token, Exception innerException)
            : base("Failure while executing callback for " + (token.Value != token.Type ? token.Value + " (" + token.Type + ")" : token.Type) + @" at line " + token.Line, innerException)
        { }
    }
}
