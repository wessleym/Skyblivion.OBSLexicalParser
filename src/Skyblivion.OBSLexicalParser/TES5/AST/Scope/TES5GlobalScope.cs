using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Factory;
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
        private readonly List<TES5FunctionCodeBlock> functions = new List<TES5FunctionCodeBlock>();
        private bool playerRefPropertyAdded = false;
        /*
        * TES5GlobalScope constructor.
        */
        public TES5GlobalScope(TES5ScriptHeader scriptHeader)
        {
            this.ScriptHeader = scriptHeader;
        }

        public void AddProperty(TES5Property property)
        {
            this.Properties.Add(property);
        }

        public void AddFunction(TES5FunctionCodeBlock functionCodeBlock)
        {
            functions.Add(functionCodeBlock);
        }

        public void AddFunctionIfNotExists(string name, Func<TES5FunctionCodeBlock> functionCodeBlockFactory)
        {
            if (!functions.Where(f => f.BlockName == name).Any())
            {
                AddFunction(functionCodeBlockFactory());
            }
        }

        public IEnumerable<string> Output => Properties.SelectMany(p => p.Output).Concat(functions.SelectMany(o => o.Output));

        private TES5Property? TryGetPropertyByName(string propertyName, bool throwException)
        {
            if (this.Properties.Any())
            {
                string propertyNameLower = propertyName.ToLower();
                string propertyNameLowerWithSuffix = TES5Property.AddPropertyNameSuffix(propertyNameLower, false);
                foreach (var property in this.Properties)
                {
                    string currentPropertyNameLower = property.Name.ToLower();
                    if (propertyNameLower == currentPropertyNameLower || propertyNameLowerWithSuffix == currentPropertyNameLower)
                    {
                        //Token found.
                        return property;
                    }
                }
            }
            if (throwException) { throw new InvalidOperationException(nameof(propertyName) + " " + propertyName + " not found."); }
            return null;
        }
        public TES5Property? TryGetPropertyByName(string propertyName)
        {
            return TryGetPropertyByName(propertyName, false);
        }
        public TES5Property GetPropertyByName(string propertyName)
        {
            return TryGetPropertyByName(propertyName, true)!;
        }

        public void AddPlayerRefPropertyIfNotExists()
        {
            if (!playerRefPropertyAdded)
            {
                const string name = TES5PlayerReference.PlayerRefName;
                TES5BasicType playerType = TES5PlayerReference.TES5TypeStatic;
                TES5Property property = TES5PropertyFactory.ConstructWithoutFormIDs(name, playerType, name);
                AddProperty(property);
                playerRefPropertyAdded = true;
            }
        }
    }
}