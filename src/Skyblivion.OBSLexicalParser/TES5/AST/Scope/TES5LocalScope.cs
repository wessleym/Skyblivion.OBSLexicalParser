using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Context;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    /*
     * TES5LocalScope represents a local scope of variables - i.e. the variables which are known in a given scope.
     * Local scope can have a parent scope ( as in - you can travel local scopes as a linked list from the leafs up to
     * the root )
     * Class TES5LocalScope
     * @package Ormin\OBSLexicalParser\TES5\AST\Scope
     */
    class TES5LocalScope : ITES5Outputtable
    {
        private TES5FunctionScope functionScope;
        private TES5LocalScope parentScope;
        private List<TES5LocalVariable> variables = new List<TES5LocalVariable>();
        /*
        * TES5LocalScope constructor.
        */
        public TES5LocalScope(TES5FunctionScope functionScope, TES5LocalScope parentScope = null)
        {
            this.functionScope = functionScope;
            this.parentScope = parentScope;
        }

        public List<string> output()
        {
            return variables.SelectMany(v => v.output()).ToList();
        }

        public void addVariable(TES5LocalVariable localVariable)
        {
            this.variables.Add(localVariable);
        }
        
        public TES5LocalScope getParentScope()
        {
            return this.parentScope;
        }

        public TES5FunctionScope getFunctionScope()
        {
            return this.functionScope;
        }

        public void setParentScope(TES5LocalScope parentScope)
        {
            this.parentScope = parentScope;
        }

        public List<TES5LocalVariable> getLocalVariables()
        {
            return this.variables;
        }

        public TES5LocalVariable getVariableByName(string name)
        {
            List<TES5LocalVariable> variables = this.getVariables();
            foreach (var variable in variables)
            {
                if (variable.getPropertyName() == name)
                {
                    return variable;
                }
            }
            return null;
        }

        public List<TES5LocalVariable> getVariables()
        {
            List<TES5LocalVariable> variables = new List<TES5LocalVariable>();
            TES5LocalScope scope = this;
            do
            {
                List<TES5LocalVariable> variablePack = scope.getLocalVariables();
                variables.AddRange(variablePack);
                scope = scope.getParentScope();
            }
            while (scope != null);
            variables.AddRange(this.functionScope.getVariables().Select(v=>v.Value));
            return variables;
        }

        public TES5LocalVariable findVariableWithMeaning(TES5LocalVariableParameterMeaning meaning)
        {
            return this.functionScope.findVariableWithMeaning(meaning);
        }
    }
}