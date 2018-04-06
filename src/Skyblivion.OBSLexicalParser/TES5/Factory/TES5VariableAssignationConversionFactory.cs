using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5VariableAssignationConversionFactory
    {
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ReferenceFactory referenceFactory;
        private TES5ValueFactory valueFactory;
        private TES5VariableAssignationFactory assignationFactory;
        private TES5BranchFactory branchFactory;
        private TES5TypeInferencer typeInferencer;
        public TES5VariableAssignationConversionFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory, TES5ValueFactory valueFactory, TES5VariableAssignationFactory assignationFactory, TES5BranchFactory branchFactory, TES5TypeInferencer typeInferencer)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
            this.valueFactory = valueFactory;
            this.assignationFactory = assignationFactory;
            this.branchFactory = branchFactory;
            this.typeInferencer = typeInferencer;
        }

        public TES5CodeChunkCollection createCodeChunk(TES4VariableAssignation chunk, TES5CodeScope codeScope,  TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5CodeChunkCollection codeChunkCollection = new TES5CodeChunkCollection();
            string referenceName = chunk.Reference.StringValue;
            ITES5Referencer reference = this.referenceFactory.createReference(referenceName, globalScope, multipleScriptsScope, codeScope.LocalScope);
            ITES5Value value = this.valueFactory.createValue(chunk.Value, codeScope, globalScope, multipleScriptsScope);
            if (reference.TES5Type == TES5BasicType.T_GLOBALVARIABLE)
            { //if the reference is in reality a global variable, we will need to convert it by creating a Reference.SetValue(value); call
                //Object call creation
                TES5ObjectCallArguments objectCallArguments = new TES5ObjectCallArguments();
                objectCallArguments.Add(value);
                TES5ObjectCall objectCall = this.objectCallFactory.CreateObjectCall(reference, "SetValue", multipleScriptsScope, objectCallArguments);
                codeChunkCollection.add(objectCall);
            }
            else
            {
                if (!reference.ReferencesTo.getPropertyType().isPrimitive() && value.TES5Type.isPrimitive())
                {
                    //Hacky!
                    TES5IntegerOrFloat valueNumber = value as TES5IntegerOrFloat;
                    if (valueNumber != null && valueNumber.ConvertedIntValue == 0)
                    {
                        value = new TES5None();
                    }
                }

                TES5VariableAssignation assignation = this.assignationFactory.createAssignation(reference, value);
                this.typeInferencer.inferenceObjectByAssignation(reference, value, multipleScriptsScope);
                codeChunkCollection.add(assignation);
                //post analysis.
                //Todo - rethink the prefix here
                ITES5Referencer referencerValue = value as ITES5Referencer;
                if (referencerValue!=null && referencerValue.Name == TES5Property.AddPropertyNameSuffix(TES5ReferenceFactory.MESSAGEBOX_VARIABLE_CONST))
                {
                    /*
                     * Create block:
                     * variable = this.TES4_MESSAGEBOX_RESULT; ; assignation
                     * if(variable != -1) ; branch, expression
                     *   this.TES4_MESSAGEBOX_RESULT = -1; ; reassignation
                     * endIf
                     */
                    TES5Integer minusOne = new TES5Integer(-1);
                    TES5ArithmeticExpression expression = TES5ExpressionFactory.createArithmeticExpression(reference, TES5ArithmeticExpressionOperator.OPERATOR_NOT_EQUAL, minusOne);
                    TES5VariableAssignation reassignation = this.assignationFactory.createAssignation(referencerValue, minusOne);
                    TES5Branch branch = this.branchFactory.createSimpleBranch(expression, codeScope.LocalScope);
                    branch.getMainBranch().getCodeScope().Add(reassignation);
                    codeChunkCollection.add(branch);
                }
            }

            return codeChunkCollection;
        }
    }
}