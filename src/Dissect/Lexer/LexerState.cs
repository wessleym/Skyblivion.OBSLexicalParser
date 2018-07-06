using Dissect.Lexer.Recognizer;
using System.Collections.Generic;

namespace Dissect.Lexer
{
    //DissectChange:
    public class LexerState
    {
        public readonly Dictionary<string, object> Actions = new Dictionary<string, object>();
        public readonly Dictionary<string, IRecognizer> Recognizers = new Dictionary<string, IRecognizer>();
        public string[] SkipTokens = new string[] { };
    }
}
