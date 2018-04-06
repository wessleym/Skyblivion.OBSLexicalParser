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
        public bool IsSpecial = false;
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

        public IEnumerable<string> Output => properties.SelectMany(p => p.Output);

        public TES5Property getPropertyByName(string propertyName)
        {
            if (this.properties.Any())
            {
                string propertyNameLower = propertyName.ToLower();
                string propertyNameLowerWithSuffix = TES5Property.AddPropertyNameSuffix(propertyNameLower, false);
                foreach (var property in this.properties)
                {
                    string currentPropertyNameLower = property.getPropertyName().ToLower();
                    if (propertyNameLower == currentPropertyNameLower || propertyNameLowerWithSuffix==currentPropertyNameLower)
                    {
                        //Token found.
                        return property;
                    }
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