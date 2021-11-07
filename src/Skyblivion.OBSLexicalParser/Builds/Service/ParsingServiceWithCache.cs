using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds.Service
{
    abstract class ParsingServiceWithCache<T> : ParsingService<T> where T : ITES4CodeFilterable
    {
        private readonly Dictionary<string, T> parsingCache = new Dictionary<string, T>();
        private readonly SyntaxErrorCleanParser parser;
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
            return parsingCache.GetOrAdd(scriptPath, () => Parse(scriptPath));
        }
    }
}