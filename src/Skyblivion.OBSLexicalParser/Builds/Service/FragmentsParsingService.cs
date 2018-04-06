using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Lexers;
using Skyblivion.OBSLexicalParser.TES4.Parsers;

namespace Skyblivion.OBSLexicalParser.Builds.Service
{
    class FragmentsParsingService : ParsingService<TES4CodeChunks>
    {
        public FragmentsParsingService(SyntaxErrorCleanParser parser)
            : base(parser)
        { }

        protected override OBScriptLexer GetLexer()
        {
            return new FragmentLexer();
        }
    }
}