using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5StaticReferenceFactory
    {
        private static TES5StaticReference Create(string name)
        {
            return new TES5StaticReference(name);
        }
        public static TES5StaticReference Create(TES5BasicType tes5BasicType)
        {
            return new TES5StaticReference(tes5BasicType.Name, overrideType: tes5BasicType);
        }

        public static TES5StaticReference Debug => Create("Debug");
        public static TES5StaticReference Game => Create("Game");
        public static TES5StaticReference Keyword => Create("Keyword");
        public static TES5StaticReference StringUtil => Create("StringUtil");
        public static TES5StaticReference Utility => Create("Utility");
        public static TES5StaticReference Weather => Create("Weather");
    }
}
