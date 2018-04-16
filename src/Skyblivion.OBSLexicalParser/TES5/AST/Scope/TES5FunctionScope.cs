using Skyblivion.ESReader.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    /*
     * Class TES5FunctionScope
     * Represents variable scope at function level.
     */
    class TES5FunctionScope
    {
        /*
        *  This might be the Event"s name, or Function"s name.
        */
        public string BlockName { get; private set; }
        public Dictionary<string, TES5LocalVariable> Variables { get; private set; } = new Dictionary<string, TES5LocalVariable>();
        /*
        * A hashmap to speedup the search
        */
        private Dictionary<TES5LocalVariableParameterMeaning, TES5LocalVariable> variablesByMeanings = new Dictionary<TES5LocalVariableParameterMeaning, TES5LocalVariable>();
        /*
        * TES5FunctionScope constructor.
        */
        public TES5FunctionScope(string blockName)
        {
            this.BlockName = blockName;
        }

        public void AddVariable(TES5LocalVariable localVariable)
        {
            this.Variables.Add(localVariable.PropertyNameWithSuffix, localVariable);
            foreach (TES5LocalVariableParameterMeaning meaning in localVariable.Meanings)
            {
                try
                {
                    this.variablesByMeanings.Add(meaning, localVariable);
                }
                catch (ArgumentException)
                {
                    throw new ConversionException("Cannot register variable " + localVariable.PropertyNameWithSuffix+ " - it has a meaning " + meaning.Name + " that was already registered before.");
                }
            }
        }

        public void Rename(string newName)
        {
            this.BlockName = newName;
        }

        public TES5LocalVariable GetVariable(string name)
        {
            return this.Variables.GetWithFallback(name, () => null);
        }

        public IEnumerable<string> GetVariablesOutput()
        {
            return this.Variables.Values.Select(v => v.PropertyType.Output.Single() + " " + v.PropertyNameWithSuffix);
        }

        public TES5LocalVariable GetVariableWithMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.variablesByMeanings.GetWithFallback(meaning, () => null);
        }
    }
}