using Dissect.Lexer.TokenStream;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Lexers;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.Service
{
    abstract class ParsingService<T> where T : ITES4CodeFilterable
    {
        private readonly Lazy<OBScriptLexer> lexerLazy;
        private readonly Lazy<SyntaxErrorCleanParser> parserLazy;
        public ParsingService()
        {
            lexerLazy = new Lazy<OBScriptLexer>(() => GetLexer());
            parserLazy = new Lazy<SyntaxErrorCleanParser>(() => GetParser());
        }

        protected abstract OBScriptLexer GetLexer();
        protected abstract SyntaxErrorCleanParser GetParser();

        public T Parse(string scriptPath)
        {
            OBScriptLexer lexer = this.lexerLazy.Value;
            string sourceText = File.ReadAllText(scriptPath);
            ArrayTokenStream tokens = lexer.LexWithFixes(sourceText);
            SyntaxErrorCleanParser parser = this.parserLazy.Value;
            T result = (T)parser.ParseWithFixLogic(tokens);
            return result;
        }
    }
}
