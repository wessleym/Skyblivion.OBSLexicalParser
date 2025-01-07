using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5StaticReference : ITES5Referencer
    {
        public string Name { get; }
        private readonly ITES5Type? overrideType;
        public TES5StaticReference(string name, ITES5Type? overrideType = null)
        {
            this.Name = name;
            this.overrideType = overrideType;
        }

        public IEnumerable<string> Output => new string[] { this.Name };

        public ITES5Type TES5Type => overrideType ?? TES5BasicType.GetFirstCaseInsensitive(this.Name);

        public ITES5VariableOrProperty? ReferencesTo => null;
    }
}