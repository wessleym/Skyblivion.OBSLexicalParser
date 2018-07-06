using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Context;
using System.Collections.Generic;
using System.Linq;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    /*
     * TES5LocalScope represents a local scope of variables - i.e. the variables which are known in a given scope.
     * Local scope can have a parent scope ( as in - you can travel local scopes as a linked list from the leafs up to
     * the root )
     * Class TES5LocalScope
     */
    class TES5LocalScope : ITES5Outputtable
    {
        public TES5FunctionScope FunctionScope { get; private set; }
        public TES5LocalScope ParentScope { get; set; }
        private List<TES5LocalVariable> LocalVariables = new List<TES5LocalVariable>();
        /*
        * TES5LocalScope constructor.
        */
        public TES5LocalScope(TES5FunctionScope functionScope, TES5LocalScope parentScope = null)
        {
            this.FunctionScope = functionScope;
            this.ParentScope = parentScope;
        }

        public IEnumerable<string> Output => LocalVariables.SelectMany(v => v.Output);

        public void AddVariable(TES5LocalVariable localVariable)
        {
            this.LocalVariables.Add(localVariable);
        }

        public TES5LocalVariable GetVariable(string name)
        {
            return this.GetAllVariables().Where(v => v.Name == name).FirstOrDefault();
        }

        private IEnumerable<TES5LocalVariable> GetAllVariables()
        {
            TES5LocalScope scope = this;
            do
            {
                foreach (TES5LocalVariable variable in scope.LocalVariables)
                {
                    yield return variable;
                }
                scope = scope.ParentScope;
            }
            while (scope != null);
            foreach (TES5LocalVariable variable in this.FunctionScope.Variables.Values)
            {
                yield return variable;
            }
        }

        public TES5LocalVariable GetVariableWithMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.FunctionScope.GetVariableWithMeaning(meaning);
        }
    }
}