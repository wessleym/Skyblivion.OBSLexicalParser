using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection
{
    class TES5GlobalVariables
    {
        private readonly Dictionary<string, TES5GlobalVariable> globalVariables;
        /*
        * TES5GlobalVariables constructor.
        */
        public TES5GlobalVariables(List<TES5GlobalVariable> globalVariables)
        {
            this.globalVariables = new Dictionary<string, TES5GlobalVariable>(StringComparer.OrdinalIgnoreCase);
            foreach (var globalVariable in globalVariables)
            {
                this.globalVariables.Add(globalVariable.Name, globalVariable);
            }
        }

        public bool ContainsName(string globalVariableName)
        {
            return this.globalVariables.ContainsKey(globalVariableName);
        }
    }
}