using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5ObjectProperty : ITES5Referencer, ITES5ObjectAccess
    {
        private ITES5Referencer objectReference;
        private TES5Property property;
        public TES5ObjectProperty(ITES5Referencer objectReference, TES5Property property)
        {
            this.objectReference = objectReference;
            this.property = property;
        }

        public IEnumerable<string> output()
        {
            string referenceOutput = this.objectReference.output().Single();
            return new string[] { referenceOutput + "." + this.property.getPropertyName() };
        }

        public ITES5Type getType()
        {
            return this.property.getPropertyType();
        }

        public ITES5Referencer getAccessedObject()
        {
            return this.objectReference;
        }

        public ITES5Variable getReferencesTo()
        {
            return this.property;
        }

        public string getName()
        {
            return this.property.getPropertyName();
        }
    }
}
