using Dissect.Lexer.TokenStream;
using Dissect.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Lexers;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System.Collections.Generic;
using System.IO;
using System;

namespace Skyblivion.OBSLexicalParser.Builds.Service
{
    abstract class ParsingService<T> where T : ITES4CodeFilterable
    {
        private Dictionary<string, T> parsingCache = new Dictionary<string, T>();
        private SyntaxErrorCleanParser parser;
        private Lazy<OBScriptLexer> lexerLazy;
        public ParsingService(SyntaxErrorCleanParser parser)
        {
            this.parser = parser;
            lexerLazy = new Lazy<OBScriptLexer>(() => GetLexer());
        }

        protected abstract OBScriptLexer GetLexer();

        public T parseScript(string scriptPath)
        {
            return parsingCache.GetOrAdd(scriptPath, () =>
            {
                OBScriptLexer lexer = lexerLazy.Value;
                string sourceText = File.ReadAllText(scriptPath);
                ArrayTokenStream tokens = lexer.LexWithFixes(sourceText);
                return (T)this.parser.ParseWithFixLogic(tokens);
            });
        }
    }
}