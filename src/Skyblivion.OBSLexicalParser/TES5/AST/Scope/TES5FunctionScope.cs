using Skyblivion.ESReader.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    /*
     * Class TES5FunctionScope
     * Represents variable scope at function level.
     */
    class TES5FunctionScope
    {
        /*
        *  Block name
        */
        private string blockName;
        private Dictionary<string, TES5LocalVariable> variables = new Dictionary<string, TES5LocalVariable>();
        /*
        * A hashmap to speedup the search
        */
        private Dictionary<TES5LocalVariableParameterMeaning, TES5LocalVariable> variablesByMeanings = new Dictionary<TES5LocalVariableParameterMeaning, TES5LocalVariable>();
        /*
        * TES5FunctionScope constructor.
        */
        public TES5FunctionScope(string blockName)
        {
            this.blockName = blockName;
        }

        public void addVariable(TES5LocalVariable localVariable)
        {
            this.variables.Add(localVariable.getPropertyName(), localVariable);
            foreach (TES5LocalVariableParameterMeaning meaning in localVariable.getMeanings())
            {
                try
                {
                    this.variablesByMeanings.Add(meaning, localVariable);
                }
                catch (ArgumentException)
                {
                    throw new ConversionException("Cannot register variable " + localVariable.getPropertyName() + " - it has a meaning " + meaning.Name + " that was already registered before.");
                }
            }
        }

        public void renameTo(string newName)
        {
            this.blockName = newName;
        }

        public TES5LocalVariable getVariableByName(string name)
        {
            return this.variables.GetWithFallback(name, () => null);
        }

        public Dictionary<string, TES5LocalVariable> getVariables()
        {
            return this.variables;
        }

        public TES5LocalVariable findVariableWithMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.variablesByMeanings.GetWithFallback(meaning, () => null);
        }

        /*
        * Get the block name. This might be the Event"s name, or Function"s name.
        */
        public string getBlockName()
        {
            return this.blockName;
        }
    }
}