using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5StaticReferenceFactory
    {
        private readonly ESMAnalyzer esmAnalyzer;
        public TES5StaticReferenceFactory(ESMAnalyzer esmAnalyzer)
        {
            this.esmAnalyzer = esmAnalyzer;
        }

        private TES5StaticReference Create(string name)
        {
            return new TES5StaticReference(name, esmAnalyzer);
        }

        public TES5StaticReference Debug => Create("Debug");
        public TES5StaticReference Game => Create("Game");
        public TES5StaticReference StringUtil => Create("StringUtil");
        public TES5StaticReference Utility => Create("Utility");
        public TES5StaticReference Weather => Create("Weather");
    }
}
