using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES4.Types;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory.Functions;
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

        private static TES5Property Construct(string name, ITES5Type propertyType, string? referenceEdid, Nullable<int> tes4FormID, Nullable<int> tes5FormID)
        {
            return new TES5Property(name, propertyType, referenceEdid, tes4FormID, tes5FormID);
        }
        public static TES5Property ConstructWithTES4FormID(string name, ITES5Type propertyType, string? referenceEdid, Nullable<int> tes4FormID)
        {
            return Construct(name, propertyType, referenceEdid, tes4FormID, null);
        }
        public static TES5Property ConstructWithTES5FormID(string name, ITES5Type propertyType, string? referenceEdid, Nullable<int> tes5FormID)
        {
            return Construct(name, propertyType, referenceEdid, null, tes5FormID);
        }
        public static TES5Property ConstructWithoutFormID(string name, ITES5Type propertyType, string? referenceEdid)
        {
            return Construct(name, propertyType, referenceEdid, null, null);
        }

        /*
        * Create an pre-defined property from a ref VariableDeclaration
        */
        private TES5Property CreatePropertyFromReference(TES4VariableDeclaration declaration, TES5GlobalVariableCollection globalVariables)
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
                    if (declaration.VariableType != TES4Type.T_REF)
                    {
                        throw new ConversionException("Unknown type:  " + declaration.VariableType.Name + " " + declaration.VariableType.Name);
                    }
                    ITES5Type? esmType = esmAnalyzer.GetTypeByEDIDWithFollow(variableName, false);//This seems to return null always, which makes sense since declaration.TES5Type is null.
                    type = esmType != null ? esmType : TES5BasicType.T_FORM;
                }
            }
            return ConstructWithTES4FormID(variableName, type, variableName, tes4FormID);
        }

        private TES5Property CreateProperty(TES4VariableDeclaration variable, TES5GlobalVariableCollection globalVariables)
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

        private TES5Property CreateAndAddProperty(TES4VariableDeclaration variable, TES5GlobalScope globalScope, TES5GlobalVariableCollection globalVariables)
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

        private void CreateAndAddComment(TES4Comment comment, TES5GlobalScope globalScope)
        {
            globalScope.AddComment(TES5CommentFactory.Construct(comment));
        }

        /*
        * @throws ConversionException
        */
        public void CreateAndAddProperties(IEnumerable<ITES4ScriptHeaderVariableDeclarationOrComment> variablesAndComments, TES5GlobalScope globalScope, TES5GlobalVariableCollection globalVariables)
        {
            Dictionary<string, Tuple<TES4VariableDeclaration, TES5Property>> alreadyDefinedVariables = new Dictionary<string, Tuple<TES4VariableDeclaration, TES5Property>>();
            foreach (ITES4ScriptHeaderVariableDeclarationOrComment variableOrComment in variablesAndComments)
            {
                TES4VariableDeclaration? variable = variableOrComment as TES4VariableDeclaration;
                if (variable != null)
                {
                    string variableName = variable.VariableName;
                    string variableNameLower = variableName.ToLower();
                    TES4Type variableType = variable.VariableType;
                    Tuple<TES4VariableDeclaration, TES5Property>? alreadyDefinedVariable;
                    TES5Comment? variableComment = variable.Comment != null ? TES5CommentFactory.Construct(variable.Comment) : null;
                    if (alreadyDefinedVariables.TryGetValue(variableNameLower, out alreadyDefinedVariable))
                    {
                        TES4Type alreadyDefinedVariableType = alreadyDefinedVariable.Item1.VariableType;
                        if (variableType == alreadyDefinedVariableType)
                        {
                            if (variableComment != null)
                            {
                                alreadyDefinedVariable.Item2.AddComment(variableComment);
                            }
                            continue; //Same variable defined twice, smack the original script developer and fallthrough silently.
                        }
                        throw GetDuplicatePropertyException(variableName, alreadyDefinedVariableType.Name, variable.VariableType.Name);
                    }

                    TES5Property property = CreateAndAddProperty(variable, globalScope, globalVariables);
                    alreadyDefinedVariables.Add(variableNameLower, new Tuple<TES4VariableDeclaration, TES5Property>(variable, property));
                    if (variableComment != null) { property.AddComment(variableComment); }
                    continue;
                }
                TES4Comment? comment = variableOrComment as TES4Comment;
                if (comment != null)
                {
                    CreateAndAddComment(comment, globalScope);
                    continue;
                }
                throw new InvalidOperationException("Unhandled " + nameof(variableOrComment) + " type:  " + variableOrComment.GetType().FullName);
            }
        }
    }
}