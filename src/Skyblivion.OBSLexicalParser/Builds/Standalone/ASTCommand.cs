using Skyblivion.OBSLexicalParser.TES4.Lexers;
using Skyblivion.OBSLexicalParser.TES4.Parsers;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class ASTCommand : ASTCommandBase
    {
        protected override TES4ObscriptCodeGrammar GetGrammar()
        {
            return new TES4OBScriptGrammar();
        }

        protected override OBScriptLexer GetLexer()
        {
            return new ScriptLexer();
        }
    }
}