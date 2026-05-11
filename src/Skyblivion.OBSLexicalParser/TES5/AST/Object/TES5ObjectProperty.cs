using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectProperty : TES5Castable, ITES5Referencer, ITES5ObjectAccess
    {
        public ITES5Referencer AccessedObject { get; }
        private readonly TES5Property property;
        public TES5ObjectProperty(ITES5Referencer objectReference, TES5Property property)
        {
            this.AccessedObject = objectReference;
            this.property = property;
        }

        public string OutputSingle
        {
            get
            {
                string referenceOutput = this.AccessedObject.OutputSingle;
                return referenceOutput + "." + this.property.Name + ManualCastToOutput;
            }
        }

        public IEnumerable<string> Output => [OutputSingle];

        public ITES5Type TES5Type => this.property.TES5Type;

        public ITES5VariableOrProperty ReferencesTo => this.property;

        public string Name => this.property.Name;
    }
}
