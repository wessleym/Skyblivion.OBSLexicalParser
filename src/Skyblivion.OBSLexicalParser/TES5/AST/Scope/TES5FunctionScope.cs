using Skyblivion.ESReader.Extensions;
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
        private readonly Dictionary<string, TES5SignatureParameter> parameters = new Dictionary<string, TES5SignatureParameter>();
        /*
        * A hashmap to speedup the search
        */
        private readonly Dictionary<TES5LocalVariableParameterMeaning, TES5SignatureParameter> parametersByMeanings = new Dictionary<TES5LocalVariableParameterMeaning, TES5SignatureParameter>();
        /*
        * TES5FunctionScope constructor.
        */
        public TES5FunctionScope(string blockName)
        {
            this.BlockName = blockName;
        }

        public void AddParameter(TES5SignatureParameter parameter)
        {
            this.parameters.Add(parameter.Name, parameter);
            foreach (TES5LocalVariableParameterMeaning meaning in parameter.Meanings)
            {
                try
                {
                    this.parametersByMeanings.Add(meaning, parameter);
                }
                catch (ArgumentException)
                {
                    throw new ConversionException("Cannot register parameter " + parameter.Name+ " - it has a meaning " + meaning.Name + " that was already registered before.");
                }
            }
        }

        public void Rename(string newName)
        {
            this.BlockName = newName;
        }

        public TES5SignatureParameter GetParameter(string name)
        {
            return this.parameters[name];
        }

        public IEnumerable<TES5SignatureParameter> GetParameters()
        {
            return this.parameters.Values;
        }

        public IEnumerable<string> GetParametersOutput()
        {
            //WTM:  Changed TES5Type to TES5DeclaredType
            return this.parameters.Values.Select(v => v.TES5DeclaredType.Output.Single() + " " + v.Name);
        }

        public TES5SignatureParameter? TryGetParameterByMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.parametersByMeanings.GetWithFallbackNullable(meaning, () => null);
        }
        public TES5SignatureParameter GetParameterByMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.parametersByMeanings[meaning];
        }
    }
}
