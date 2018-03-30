using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    class TES5GlobalScope : ITES5Outputtable
    {
        private TES5ScriptHeader scriptHeader;
        private List<TES5Property> properties = new List<TES5Property>();
        /*
        * TES5GlobalScope constructor.
        */
        public TES5GlobalScope(TES5ScriptHeader scriptHeader)
        {
            this.scriptHeader = scriptHeader;
        }

        public TES5ScriptHeader getScriptHeader()
        {
            return this.scriptHeader;
        }

        public void add(TES5Property declaration)
        {
            this.properties.Add(declaration);
        }

        public IEnumerable<string> output()
        {
            return properties.SelectMany(p => p.output());
        }

        public TES5Property getPropertyByName(string propertyName)
        {
            foreach (var property in this.properties)
            {
                if (propertyName.ToLower()+"_p" == property.getPropertyName().ToLower())
                {
                    //Token found.
                    return property;
                }
            }
            return null;
        }

        public List<TES5Property> getPropertiesList()
        {
            return this.properties;
        }
    }
}