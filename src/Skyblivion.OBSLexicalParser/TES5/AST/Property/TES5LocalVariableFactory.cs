using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    internal static class TES5LocalVariableFactory
    {
        public static TES5LocalVariable Create(TES5BasicType type, string name)
        {
            return new TES5LocalVariable(name, type);
        }

        public static TES5Reference CreateWithInitialization(TES5BasicType variableType, string variableName, ITES5Value initialValue, ITES5ScopeWithVariablesAddable codeScope)
        {
            TES5LocalVariable variable = Create(variableType, variableName);
            TES5Reference referenceToVariable = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(variable);
            TES5LocalVariableInitialization initialization = new TES5LocalVariableInitialization(variable, TES5VariableAssignationFactory.CreateAssignation(referenceToVariable, initialValue));
            codeScope.AddVariable(initialization);
            return referenceToVariable;
        }

        public static TES5Reference CreateWithInitialization(TES5BasicType variableType, string variableName, ITES5VariableOrProperty initialValue, ITES5ScopeWithVariablesAddable codeScope)
        {
            TES5Reference referenceToVariableOrProperty = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(initialValue);
            return CreateWithInitialization(variableType, variableName, referenceToVariableOrProperty, codeScope);
        }
    }
}
