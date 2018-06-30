using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5StaticReference : ITES5Referencer
    {
        public string Name { get; private set; }
        private TES5StaticReference(string name)
        {
            this.Name = name;
        }

        public IEnumerable<string> Output => new string[] { this.Name };

        public ITES5Type TES5Type => TES5TypeFactory.MemberByValue(this.Name);

        public ITES5Variable ReferencesTo => null;

        public static TES5StaticReference Debug => new TES5StaticReference("Debug");
        public static TES5StaticReference Game => new TES5StaticReference("Game");
        public static TES5StaticReference StringUtil => new TES5StaticReference("StringUtil");
        public static TES5StaticReference Utility => new TES5StaticReference("Utility");
        public static TES5StaticReference Weather => new TES5StaticReference("Weather");
    }
}