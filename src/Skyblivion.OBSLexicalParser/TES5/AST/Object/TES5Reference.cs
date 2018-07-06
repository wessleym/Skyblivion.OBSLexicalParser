using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5Reference : TES5Castable, ITES5Referencer
    {
        public ITES5VariableOrProperty ReferencesTo { get; private set; }
        public TES5Reference(ITES5VariableOrProperty referencesTo)
        {
            this.ReferencesTo = referencesTo;
        }

        public IEnumerable<string> Output => new string[] { this.ReferencesTo.Name+ ManualCastToOutput };

        public string Name => this.ReferencesTo.Name;

        public ITES5Type TES5Type => this.ReferencesTo.TES5Type;
    }
}