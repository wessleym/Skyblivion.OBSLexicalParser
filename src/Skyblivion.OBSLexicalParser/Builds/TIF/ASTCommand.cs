using Dissect.Lexer.TokenStream;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.Lexer;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class ASTCommand : Skyblivion.OBSLexicalParser.Builds.IASTCommand
    {
        private SyntaxErrorCleanParser parser;
        public void initialize()
        {
            parser = new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar());
        }

        public TES4Script getAST(string sourcePath)
        {
            FragmentLexer lexer = new FragmentLexer();
            ArrayTokenStream tokens = lexer.lex(File.ReadAllText(sourcePath));
            TES4Script AST = this.parser.ParseAs<TES4Script>(tokens);
            return AST;
        }
    }
}