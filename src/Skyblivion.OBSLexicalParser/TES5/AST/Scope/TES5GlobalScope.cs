using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    class TES5GlobalScope : ITES5Outputtable
    {
        public TES5ScriptHeader ScriptHeader { get; private set; }
        public List<TES5Property> Properties { get; private set; } = new List<TES5Property>();
        /*
        * TES5GlobalScope constructor.
        */
        public TES5GlobalScope(TES5ScriptHeader scriptHeader)
        {
            this.ScriptHeader = scriptHeader;
        }

        public void Add(TES5Property declaration)
        {
            this.Properties.Add(declaration);
        }

        public IEnumerable<string> Output => Properties.SelectMany(p => p.Output);

        public TES5Property getPropertyByName(string propertyName)
        {
            if (this.Properties.Any())
            {
                string propertyNameLower = propertyName.ToLower();
                string propertyNameLowerWithSuffix = TES5Property.AddPropertyNameSuffix(propertyNameLower, false);
                foreach (var property in this.Properties)
                {
                    string currentPropertyNameLower = property.PropertyNameWithSuffix.ToLower();
                    if (propertyNameLower == currentPropertyNameLower || propertyNameLowerWithSuffix==currentPropertyNameLower)
                    {
                        //Token found.
                        return property;
                    }
                }
            }
            return null;
        }
    }
}