using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Context;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public TES5FunctionScope FunctionScope { get; }
        public TES5LocalScope? ParentScope { get; set; }
        private readonly List<TES5LocalVariable> localVariables = new List<TES5LocalVariable>();
        /*
        * TES5LocalScope constructor.
        */
        public TES5LocalScope(TES5FunctionScope functionScope, TES5LocalScope? parentScope = null)
        {
            this.FunctionScope = functionScope;
            this.ParentScope = parentScope;
        }

        public IEnumerable<string> Output => localVariables.SelectMany(v => v.Output);

        public void AddVariable(TES5LocalVariable localVariable)
        {
            this.localVariables.Add(localVariable);
        }

        public ITES5VariableOrProperty? TryGetVariable(string name)
        {
            return this.GetAllVariables().Where(v => v.Name == name).FirstOrDefault();
        }
        public ITES5VariableOrProperty GetVariable(string name)
        {
            ITES5VariableOrProperty? variable = TryGetVariable(name);
            if (variable != null) { return variable; }
            throw new InvalidOperationException(nameof(variable) + " was null when " + nameof(name) + " was " + name + ".");
        }

        private IEnumerable<TES5LocalVariable> GetScopeVariables()
        {
            TES5LocalScope? scope = this;
            do
            {
                foreach (TES5LocalVariable variable in scope.localVariables)
                {
                    yield return variable;
                }
                scope = scope.ParentScope;
            }
            while (scope != null);
        }

        /**
         * TODO - Maybe we ought to create a new interface marking locally
         * scoped variables and have TES5LocalVariable and TES5SignatureParameter
         * implement it?
         */
        private IEnumerable<ITES5VariableOrProperty> GetAllVariables()
        {
            return GetScopeVariables().Cast<ITES5VariableOrProperty>().Concat(this.FunctionScope.GetParameters());
        }

        public TES5SignatureParameter? TryGetFunctionParameterByMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.FunctionScope.TryGetParameterByMeaning(meaning);
        }
        public TES5SignatureParameter GetVariableWithMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.FunctionScope.GetParameterByMeaning(meaning);
        }

        public void CopyVariablesFrom(TES5LocalScope otherScope)
        {
            TES5LocalVariable[] scopeVariables = GetScopeVariables().ToArray();
            TES5LocalVariable[] otherScopeVariables = otherScope.localVariables.Where(v => !scopeVariables.Contains(v)).ToArray();
            foreach (var variable in otherScopeVariables)
            {
                localVariables.Add(variable);
            }
        }
    }
}