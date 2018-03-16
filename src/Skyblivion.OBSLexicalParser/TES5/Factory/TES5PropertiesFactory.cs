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
    class TES5PropertiesFactory
    {
        /*
        * Create an pre-defined property from a ref VariableDeclaration
        */
        private TES5Property createPropertyFromReference(TES4VariableDeclaration declaration, TES5GlobalVariables globalVariables)
        {
            if (globalVariables.hasGlobalVariable(declaration.getVariableName()))
            {
                return new TES5Property(declaration.getVariableName(), TES5BasicType.T_GLOBALVARIABLE, declaration.getVariableName());
            }
            else
            {
                return new TES5Property(declaration.getVariableName(), TES5BasicType.T_FORM, declaration.getVariableName());
            }
        }

        /*
             * @throws ConversionException
        */
        public void createProperties(TES4VariableDeclarationList variableList, TES5GlobalScope globalScope, TES5GlobalVariables globalVariables)
        {
            /*
             * @var TES4VariableDeclaration[] alreadyDefinedVariables
             */
            Dictionary<string, TES4VariableDeclaration> alreadyDefinedVariables = new Dictionary<string, TES4VariableDeclaration>();
            foreach (TES4VariableDeclaration variable in variableList.getVariableList())
            {
                string variableName = variable.getVariableName();
                string variableNameLower = variableName.ToLower();
                TES4Type variableType = variable.getVariableType();
                if (alreadyDefinedVariables.ContainsKey(variableNameLower))
                {
                    if (variableType == alreadyDefinedVariables[variableNameLower].getVariableType())
                    {
                        continue; //Same variable defined twice, smack the original script developer and fallthrough silently.
                    }

                    throw new ConversionException("Double definition of variable named " + variableName + " with different types ( " + alreadyDefinedVariables[variableNameLower].getVariableType().Name + " and " + variable.getVariableType().Name + " )");
                }

                TES5Property property;
                if (variableType == TES4Type.T_FLOAT)
                {
                    property = new TES5Property(variable.getVariableName(), TES5BasicType.T_FLOAT, null);
                }
                else if (variableType == TES4Type.T_INT || variableType == TES4Type.T_SHORT || variableType == TES4Type.T_LONG)
                {
                    property = new TES5Property(variable.getVariableName(), TES5BasicType.T_INT, null);
                }
                else if (variableType == TES4Type.T_REF)
                {
                    property = this.createPropertyFromReference(variable, globalVariables);
                }
                else
                {
                    throw new ConversionException("Unknown variable declaration type.");
                }

                globalScope.add(property);
                alreadyDefinedVariables[variableNameLower] = variable;
            }
        }
    }
}