using Dissect.Lexer.TokenStream;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.Lexer;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class ASTCommand : IASTCommand
    {
        private SyntaxErrorCleanParser parser;
        public void initialize()
        {
            parser = new SyntaxErrorCleanParser(new TES4OBScriptGrammar());
        }

        public TES4Script getAST(string sourcePath)
        {
            ScriptLexer lexer = new ScriptLexer();
            ArrayTokenStream tokens = lexer.lex(File.ReadAllText(sourcePath));
            TES4Script AST = (TES4Script)this.parser.ParseWithFixLogic(tokens);
            return AST;
        }
    }
}