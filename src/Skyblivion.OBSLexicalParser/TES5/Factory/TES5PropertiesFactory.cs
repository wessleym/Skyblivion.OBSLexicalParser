using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    static class TES5PropertiesFactory
    {
        /*
        * Create an pre-defined property from a ref VariableDeclaration
        */
        private static TES5Property CreatePropertyFromReference(TES4VariableDeclaration declaration, TES5GlobalVariables globalVariables)
        {
            string variableName = declaration.VariableName;
            if (globalVariables.ContainsName(variableName))
            {
                return new TES5Property(variableName, TES5BasicType.T_GLOBALVARIABLE, variableName);
            }
            else
            {
                return new TES5Property(variableName, TES5BasicType.T_FORM, variableName);
            }
        }

        /*
        * @throws ConversionException
        */
        public static void CreateProperties(TES4VariableDeclarationList variableList, TES5GlobalScope globalScope, TES5GlobalVariables globalVariables)
        {
            Dictionary<string, TES4VariableDeclaration> alreadyDefinedVariables = new Dictionary<string, TES4VariableDeclaration>();
            foreach (TES4VariableDeclaration variable in variableList.VariableList)
            {
                string variableName = variable.VariableName;
                string variableNameLower = variableName.ToLower();
                TES4Type variableType = variable.VariableType;
                TES4VariableDeclaration alreadyDefinedVariable;
                if (alreadyDefinedVariables.TryGetValue(variableNameLower, out alreadyDefinedVariable))
                {
                    if (variableType == alreadyDefinedVariable.VariableType)
                    {
                        continue; //Same variable defined twice, smack the original script developer and fallthrough silently.
                    }
                    throw new ConversionException("Double definition of variable named " + variableName + " with different types ( " + alreadyDefinedVariables[variableNameLower].VariableType.Name + " and " + variable.VariableType.Name + " )");
                }

                TES5Property property;
                if (variableType == TES4Type.T_FLOAT)
                {
                    property = new TES5Property(variable.VariableName, TES5BasicType.T_FLOAT, null);
                }
                else if (variableType == TES4Type.T_INT || variableType == TES4Type.T_SHORT || variableType == TES4Type.T_LONG)
                {
                    property = new TES5Property(variable.VariableName, TES5BasicType.T_INT, null);
                }
                else if (variableType == TES4Type.T_REF)
                {
                    property = CreatePropertyFromReference(variable, globalVariables);
                }
                else
                {
                    throw new ConversionException("Unknown variable declaration type.");
                }

                globalScope.AddProperty(property);
                alreadyDefinedVariables.Add(variableNameLower, variable);
            }
        }
    }
}