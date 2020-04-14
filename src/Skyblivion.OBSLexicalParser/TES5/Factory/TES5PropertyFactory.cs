using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5PropertyFactory
    {
        private readonly ESMAnalyzer esmAnalyzer;
        public TES5PropertyFactory(ESMAnalyzer esmAnalyzer)
        {
            this.esmAnalyzer = esmAnalyzer;
        }

        /*
        * Create an pre-defined property from a ref VariableDeclaration
        */
        private TES5Property CreatePropertyFromReference(TES4VariableDeclaration declaration, TES5GlobalVariables globalVariables)
        {
            string variableName = declaration.VariableName;
            ITES5Type type;
            if (globalVariables.ContainsName(variableName)) { type = TES5BasicType.T_GLOBALVARIABLE; }
            else
            {
                //type = TES5BasicType.T_FORM;
                //WTM:  Change:  I commented the above and added the below:
                ITES5Type? esmType = esmAnalyzer.GetTypeByEDIDWithFollow(variableName, TypeMapperMode.CompatibilityForPropertyFactory);
                type = esmType != null ? esmType : TES5BasicType.T_FORM;
            }
            return new TES5Property(variableName, type, variableName);
        }

        /*
        * @throws ConversionException
        */
        public void CreateProperties(TES4VariableDeclarationList variableList, TES5GlobalScope globalScope, TES5GlobalVariables globalVariables)
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