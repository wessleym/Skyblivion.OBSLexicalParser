using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds
{
    abstract class ASTCommandBase : ParsingService<IEnumerable<ITES4CodeChunk>>, IASTCommand<IEnumerable<ITES4CodeChunk>>
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
