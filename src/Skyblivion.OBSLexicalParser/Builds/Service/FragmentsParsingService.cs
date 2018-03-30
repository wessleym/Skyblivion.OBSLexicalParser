using Dissect.Lexer.TokenStream;
using Dissect.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Lexers;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.Service
{
    /*
     * Class FragmentsParsingService
     */
    class FragmentsParsingService
    {
        private Dictionary<string, TES4CodeChunks> parsingCache = new Dictionary<string, TES4CodeChunks>();
        private SyntaxErrorCleanParser parser;
        /*
        * Forcing implementation on purpose.
        * StandaloneParsingService constructor.
        */
        public FragmentsParsingService(SyntaxErrorCleanParser parser)
        {
            this.parser = parser;
        }

        public TES4CodeChunks parseScript(string scriptPath)
        {
            return parsingCache.GetOrAdd(scriptPath, () =>
            {
                FragmentLexer lexer = new FragmentLexer();
                ArrayTokenStream tokens = lexer.lex(File.ReadAllText(scriptPath));
                return (TES4CodeChunks)this.parser.ParseWithFixLogic(tokens);
            });
        }
    }
}