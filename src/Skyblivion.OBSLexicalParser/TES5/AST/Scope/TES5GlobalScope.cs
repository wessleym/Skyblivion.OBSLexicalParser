using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    class TES5GlobalScope : ITES5Outputtable
    {
        public TES5ScriptHeader ScriptHeader { get; private set; }
        public List<TES5Property> Properties { get; private set; } = new List<TES5Property>();
        private List<TES5FunctionCodeBlock> functions = new List<TES5FunctionCodeBlock>();
        private Lazy<TES5PlayerReference> playerRefLazy;
        public TES5PlayerReference PlayerRef => playerRefLazy.Value;
        /*
        * TES5GlobalScope constructor.
        */
        public TES5GlobalScope(TES5ScriptHeader scriptHeader)
        {
            this.ScriptHeader = scriptHeader;
            this.playerRefLazy = new Lazy<TES5PlayerReference>(() =>
            {
                const string name = TES5PlayerReference.PlayerRefName;
                AddProperty(new TES5Property(name, TES5BasicType.T_ACTOR, name, isPlayerRef: true));
                return new TES5PlayerReference();
            });
        }

        public void AddProperty(TES5Property property)
        {
            this.Properties.Add(property);
        }

        public void AddFunctionIfNotExists(string name, Func<TES5FunctionCodeBlock> functionCodeBlockFactory)
        {
            if (!functions.Where(f => f.BlockName == name).Any())
            {
                functions.Add(functionCodeBlockFactory());
            }
        }

        public IEnumerable<string> Output => Properties.SelectMany(p => p.Output).Concat(functions.SelectMany(o => o.Output));

        public TES5Property GetPropertyByName(string propertyName)
        {
            if (this.Properties.Any())
            {
                string propertyNameLower = propertyName.ToLower();
                string propertyNameLowerWithSuffix = TES5Property.AddPropertyNameSuffix(propertyNameLower, false);
                foreach (var property in this.Properties)
                {
                    string currentPropertyNameLower = property.Name.ToLower();
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