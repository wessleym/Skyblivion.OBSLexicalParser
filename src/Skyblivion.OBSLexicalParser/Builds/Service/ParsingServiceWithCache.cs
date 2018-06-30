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
    abstract class ParsingServiceWithCache<T> : ParsingService<T> where T : ITES4CodeFilterable
    {
        private Dictionary<string, T> parsingCache = new Dictionary<string, T>();
        private SyntaxErrorCleanParser parser;
        public ParsingServiceWithCache(SyntaxErrorCleanParser parser)
            : base()
        {
            this.parser = parser;
        }

        protected override SyntaxErrorCleanParser GetParser()
        {
            return this.parser;
        }

        public T ParseOrGetFromCache(string scriptPath)
        {
            return parsingCache.GetOrAdd(scriptPath, () => (T)Parse(scriptPath));
        }
    }
}