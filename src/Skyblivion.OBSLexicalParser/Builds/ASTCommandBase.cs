using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Parsers;

namespace Skyblivion.OBSLexicalParser.Builds
{
    abstract class ASTCommandBase : ParsingService<ITES4CodeFilterable>, IASTCommand
    {
        public ASTCommandBase()
            : base()
        { }

        protected abstract TES4ObscriptCodeGrammar GetGrammar();

        protected override SyntaxErrorCleanParser GetParser()
        {
            return new SyntaxErrorCleanParser(GetGrammar());
        }
    }
}
