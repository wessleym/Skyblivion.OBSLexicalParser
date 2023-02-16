using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.Lexers;
using Skyblivion.OBSLexicalParser.TES4.Parsers;

namespace Skyblivion.OBSLexicalParser.Builds.Service
{
    /*
     * Class StandaloneParsingService
     *
     * This class is meant only to be a cache layer for parsing a script
     *
     * It was created because both BuildScopeCommand and TranspileCommand need the parsed TES4Script and we didn't
     * want to parse twice.
     */
    class StandaloneParsingService : ParsingServiceWithCache<TES4Script>
    {
        public StandaloneParsingService(SyntaxErrorCleanParser parser)
            : base(parser)
        { }

        protected override OBScriptLexer GetLexer()
        {
            return new ScriptLexer();
        }
    }
}