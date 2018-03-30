using Skyblivion.OBSLexicalParser.TES4.Lexers;
using Skyblivion.OBSLexicalParser.TES4.Parsers;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
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