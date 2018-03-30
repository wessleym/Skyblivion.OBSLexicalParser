using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Object
{
    class TES5StaticReference : ITES5Referencer
    {
        private string name;
        public TES5StaticReference(string name)
        {
            this.name = name;
        }

        public IEnumerable<string> output()
        {
            return new string[] { this.name };
        }

        public string getName()
        {
            return this.name;
        }

        public ITES5Type getType()
        {
            return TES5TypeFactory.memberByValue(this.getName());
        }

        public ITES5Variable getReferencesTo()
        {
            return null;
        }
    }
}