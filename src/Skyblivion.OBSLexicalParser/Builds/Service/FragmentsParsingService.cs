using Dissect.Lexer.TokenStream;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Lexer;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.Service
{
    /*
     * Class FragmentsParsingService
     *
     * @package Ormin\OBSLexicalParser\Builds\Service
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
            if (!parsingCache.ContainsKey(scriptPath))
            {
                FragmentLexer lexer = new FragmentLexer();
                ArrayTokenStream tokens = lexer.lex(File.ReadAllText(scriptPath));
                this.parsingCache.Add(scriptPath, this.parser.ParseAs<TES4CodeChunks>(tokens));
            }
            return this.parsingCache[scriptPath];
        }
    }
}