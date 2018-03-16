using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5Reference : ITES5Referencer
    {
        private ITES5Variable referencesTo;
        /*
        * Used only for Float . int cast
        * Hacky. Should be removed at some point.
        */
        private ITES5Type manualCastTo = null;
        public TES5Reference(ITES5Variable referencesTo)
        {
            this.referencesTo = referencesTo;
        }

        public List<string> output()
        {
            if (this.manualCastTo != null)
            {
                return new List<string>() { this.referencesTo.getPropertyName() + " as " + this.manualCastTo.value() };
            }
            return new List<string>() { this.referencesTo.getPropertyName() };
        }

        public ITES5Variable getReferencesTo()
        {
            return this.referencesTo;
        }

        public string getName()
        {
            return this.referencesTo.getPropertyName();
        }

        public ITES5Type getType()
        {
            return this.referencesTo.getPropertyType();
        }

        public void setManualCastTo(ITES5Type type)
        {
            this.manualCastTo = type;
        }
    }
}