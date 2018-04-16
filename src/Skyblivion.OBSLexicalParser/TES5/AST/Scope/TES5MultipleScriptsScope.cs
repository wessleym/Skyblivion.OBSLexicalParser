using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Scope
{
    /*
     * Defines a scope of multiple scripts
     * Under this scope, things can interact together , send type-related information between scripts etc.
     * Also holds global variables list which are registered under these scripts
     * Class TES5MultipleScriptsScope
     */
    class TES5MultipleScriptsScope
    {
        private Dictionary<string, TES5GlobalScope> globalScopes;
        private TES5GlobalVariables globalVariables;
        public TES5MultipleScriptsScope(IEnumerable<TES5GlobalScope> globalScopes, TES5GlobalVariables globalVariables)
        {
            this.globalScopes = globalScopes.ToDictionary(x => x.ScriptHeader.OriginalScriptName.ToLower(), x => x);
            this.globalVariables = globalVariables;
        }

        public TES5ScriptHeader getScriptHeaderOfScript(string scriptName)
        {
            TES5GlobalScope globalScope;
            if (!this.globalScopes.TryGetValue(scriptName.ToLower(), out globalScope))
            {
                throw new ConversionException(nameof(TES5MultipleScriptsScope)+"."+nameof(getScriptHeaderOfScript) +":  Cannot find a global scope for script " + scriptName + " - make sure that the multiple scripts scope is built correctly.");
            }
            return globalScope.ScriptHeader;
        }

        public TES5Property getPropertyFromScript(string scriptName, string propertyName)
        {
            TES5GlobalScope globalScope;
            if (!this.globalScopes.TryGetValue(scriptName.ToLower(), out globalScope))
            {
                throw new ConversionException(nameof(TES5MultipleScriptsScope) + "." + nameof(getPropertyFromScript) + ": - Cannot find a global scope for script " + scriptName+" - make sure that the multiple scripts scope is built correctly.");
            }
            TES5Property property = globalScope.getPropertyByName(propertyName);
            if (property == null)
            {
                throw new ConversionException(nameof(TES5MultipleScriptsScope) + "." + nameof(getPropertyFromScript) + ": - Cannot find a property " + propertyName+" in script name "+scriptName);
            }
            return property;
        }

        public bool hasGlobalVariable(string globalVariableName)
        {
            return this.globalVariables.hasGlobalVariable(globalVariableName);
        }
    }
}