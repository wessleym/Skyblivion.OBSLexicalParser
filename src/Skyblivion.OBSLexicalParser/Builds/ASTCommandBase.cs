using Dissect.Lexer.TokenStream;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Lexer;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds
{
    abstract class ASTCommandBase : IASTCommand
    {
        private SyntaxErrorCleanParser parser;
        public void initialize()
        {
            parser = new SyntaxErrorCleanParser(GetGrammar());
        }

        protected abstract TES4ObscriptCodeGrammar GetGrammar();

        protected abstract OBScriptLexer GetLexer();

        public ITES4CodeFilterable getAST(string sourcePath)
        {
            OBScriptLexer lexer = GetLexer();
            string sourceText = File.ReadAllText(sourcePath);
            ArrayTokenStream tokens = lexer.lex(sourceText);
            ITES4CodeFilterable AST = (ITES4CodeFilterable)this.parser.ParseWithFixLogic(tokens);
            return AST;
        }
    }
}
