using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
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

        public static TES5Property Construct(string name, ITES5Type propertyType, string? referenceEdid, Nullable<int> tes4FormID)
        {
            return new TES5Property(name, propertyType, referenceEdid, tes4FormID);
        }
        public static TES5Property ConstructWithoutFormID(string name, ITES5Type propertyType, string? referenceEdid)
        {
            return Construct(name, propertyType, referenceEdid, null);
        }

        /*
        * Create an pre-defined property from a ref VariableDeclaration
        */
        private TES5Property CreatePropertyFromReference(TES4VariableDeclaration declaration, TES5GlobalVariables globalVariables)
        {
            string variableName = declaration.VariableName;
            Nullable<int> tes4FormID = null;
            ITES5Type type;
            if (globalVariables.ContainsName(variableName)) { type = TES5BasicType.T_GLOBALVARIABLE; }
            else
            {
                tes4FormID = declaration.FormID;
                if (declaration.TES5Type != null)
                {
                    type = declaration.TES5Type;
                }
                else
                {
                    ITES5Type? esmType = esmAnalyzer.GetTypeByEDIDWithFollow(variableName, false);//This seems to return null always, which makes sense since declaration.TES5Type is null.
                    type = esmType != null ? esmType : TES5BasicType.T_FORM;
                }
            }
            return Construct(variableName, type, variableName, tes4FormID);
        }

        private TES5Property CreateProperty(TES4VariableDeclaration variable, TES5GlobalVariables globalVariables)
        {
            string variableName = variable.VariableName;
            TES4Type variableType = variable.VariableType;
            if (variableType == TES4Type.T_FLOAT)
            {
                return ConstructWithoutFormID(variableName, TES5BasicType.T_FLOAT, null);
            }
            if (variableType == TES4Type.T_INT || variableType == TES4Type.T_SHORT || variableType == TES4Type.T_LONG)
            {
                return ConstructWithoutFormID(variableName, TES5BasicType.T_INT, null);
            }
            if (variableType == TES4Type.T_REF)
            {
                return CreatePropertyFromReference(variable, globalVariables);
            }
            throw new ConversionException("Unknown variable declaration type.");
        }

        private static ConversionException GetDuplicatePropertyException(string variableName, string existingType, string newType)
        {
            return new ConversionException("Double definition of variable named " + variableName + " with different types ( " + existingType + " and " + newType + " )");
        }

        public TES5Property CreateAndAddProperty(TES4VariableDeclaration variable, TES5GlobalScope globalScope, TES5GlobalVariables globalVariables)
        {
            TES5Property property = CreateProperty(variable, globalVariables);
            TES5Property? existingPropertyWithDifferentType = globalScope.Properties.Where(p => p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase) && p.TES5DeclaredType != property.TES5DeclaredType).FirstOrDefault();
            if (existingPropertyWithDifferentType != null)
            {
                throw GetDuplicatePropertyException(existingPropertyWithDifferentType.Name, existingPropertyWithDifferentType.TES5DeclaredType.OriginalName, property.TES5DeclaredType.OriginalName);
            }
            globalScope.AddProperty(property);
            return property;
        }

        /*
        * @throws ConversionException
        */
        public void CreateAndAddProperties(TES4VariableDeclarationList variableList, TES5GlobalScope globalScope, TES5GlobalVariables globalVariables)
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
                    throw GetDuplicatePropertyException(variableName, alreadyDefinedVariable.VariableType.Name, variable.VariableType.Name);
                }

                CreateAndAddProperty(variable, globalScope, globalVariables);
                alreadyDefinedVariables.Add(variableNameLower, variable);
            }
        }
    }
}