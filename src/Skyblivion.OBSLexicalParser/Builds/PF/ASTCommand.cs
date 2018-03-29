using Skyblivion.OBSLexicalParser.TES4.Lexer;
using Skyblivion.OBSLexicalParser.TES4.Parsers;

namespace Skyblivion.OBSLexicalParser.Builds.PF
{
    class ASTCommand : ASTCommandBase
    {
        protected override TES4ObscriptCodeGrammar GetGrammar()
        {
            return new TES4ObscriptCodeGrammar();
        }

        protected override OBScriptLexer GetLexer()
        {
            return new FragmentLexer();
        }
    }
}