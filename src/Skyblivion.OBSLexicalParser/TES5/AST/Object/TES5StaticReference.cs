using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5StaticReference : ITES5Referencer
    {
        public string Name { get; private set; }
        public TES5StaticReference(string name)
        {
            this.Name = name;
        }

        public IEnumerable<string> Output => new string[] { this.Name };

        public ITES5Type TES5Type => TES5TypeFactory.memberByValue(this.Name);

        public ITES5Variable ReferencesTo => null;
    }
}