using Dissect.Lexer.TokenStream;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.Lexer;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.Service
{
    /*
     * Class StandaloneParsingService
     *
     * This class is meant only to be a cache layer for parsing a script
     *
     * It was created because both BuildScopeCommand and TranspileCommand need the parsed TES4Script and we didn"t
     * want to parse twice.
     *
     * @package Ormin\OBSLexicalParser\Builds\Service
     */
    class StandaloneParsingService
    {
        private Dictionary<string, TES4Script> parsingCache = new Dictionary<string, TES4Script>();
        private SyntaxErrorCleanParser parser;
        /*
        * Forcing implementation on purpose.
         * StandaloneParsingService constructor.
        */
        public StandaloneParsingService(SyntaxErrorCleanParser parser)
        {
            this.parser = parser;
        }

        public TES4Script parseScript(string scriptPath)
        {
            if (!parsingCache.ContainsKey(scriptPath))
            {
                ScriptLexer lexer = new ScriptLexer();
                ArrayTokenStream tokens = lexer.lex(File.ReadAllText(scriptPath));
                this.parsingCache[scriptPath] = this.parser.ParseAs<TES4Script>(tokens);
            }

            return this.parsingCache[scriptPath];
        }
    }
}