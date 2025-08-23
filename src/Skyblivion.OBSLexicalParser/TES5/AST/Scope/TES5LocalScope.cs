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
    class TES5LocalScope : ITES5ScopeWithVariablesAddable, ITES5Outputtable
    {
        public TES5FunctionScope FunctionScope { get; }
        public TES5LocalScope? ParentScope { get; set; }
        private readonly List<ITES5LocalVariable> localVariables = new List<ITES5LocalVariable>();
        /*
        * TES5LocalScope constructor.
        */
        public TES5LocalScope(TES5FunctionScope functionScope, TES5LocalScope? parentScope = null)
        {
            this.FunctionScope = functionScope;
            this.ParentScope = parentScope;
        }

        public IEnumerable<string> Output => localVariables.SelectMany(v => v.Output);

        public void AddVariable(ITES5LocalVariable localVariable)
        {
            this.localVariables.Add(localVariable);
        }

        public ITES5LocalVariable? TryGetVariable(string name)
        {
            return this.GetScopeVariables().Where(v => v.Name == name).FirstOrDefault();
        }

        public ITES5VariableOrProperty? TryGetVariableOrParameter(string name)
        {
            return this.GetAllVariablesAndParameters().Where(v => v.Name == name).FirstOrDefault();
        }
        public ITES5VariableOrProperty GetVariableOrParameter(string name)
        {
            ITES5VariableOrProperty? variable = TryGetVariableOrParameter(name);
            if (variable != null) { return variable; }
            throw new InvalidOperationException(nameof(variable) + " was null when " + nameof(name) + " was " + name + ".");
        }

        private IEnumerable<TES5LocalScope> GetThisAndParentScopes()
        {
            TES5LocalScope? parentScope = this;
            while (parentScope != null)
            {
                yield return parentScope;
                parentScope = parentScope.ParentScope;
            }
        }

        private IEnumerable<ITES5LocalVariable> GetScopeVariables()
        {
            return GetThisAndParentScopes().SelectMany(s => s.localVariables);
        }

        public TES5LocalScope GetUltimateParentScope()
        {
            return GetThisAndParentScopes().Last();
        }

        /**
         * TODO - Maybe we ought to create a new interface marking locally
         * scoped variables and have TES5LocalVariable and TES5SignatureParameter
         * implement it?
         */
        private IEnumerable<ITES5VariableOrProperty> GetAllVariablesAndParameters()
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
            ITES5LocalVariable[] scopeVariables = GetScopeVariables().ToArray();
            ITES5LocalVariable[] otherScopeVariables = otherScope.localVariables.Where(v => !scopeVariables.Contains(v)).ToArray();
            foreach (var variable in otherScopeVariables)
            {
                localVariables.Add(variable);
            }
        }
    }
}