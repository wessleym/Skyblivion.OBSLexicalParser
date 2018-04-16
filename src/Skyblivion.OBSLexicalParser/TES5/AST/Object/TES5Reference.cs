using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5Reference : TES5Castable, ITES5Referencer
    {
        public ITES5Variable ReferencesTo { get; private set; }
        public TES5Reference(ITES5Variable referencesTo)
        {
            this.ReferencesTo = referencesTo;
        }

        public IEnumerable<string> Output => new string[] { this.ReferencesTo.PropertyNameWithSuffix+ ManualCastToOutput };

        public string Name => this.ReferencesTo.PropertyNameWithSuffix;

        public ITES5Type TES5Type => this.ReferencesTo.PropertyType;
    }
}