using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5SelfReference : ITES5Referencer
    {
        private TES5ScriptAsVariable scriptAsVariable;
        public TES5SelfReference(TES5ScriptAsVariable scriptAsVariable)
        {
            this.scriptAsVariable = scriptAsVariable;
        }

        public IEnumerable<string> output()
        {
            return new string[] { "self" };
        }

        public string getName()
        {
            return "self";
        }

        public ITES5Variable getReferencesTo()
        {
            return this.scriptAsVariable;
        }

        public ITES5Type getType()
        {
            return this.scriptAsVariable.getPropertyType();
        }
    }
}