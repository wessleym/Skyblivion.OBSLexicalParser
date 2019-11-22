using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5VariableAssignationConversionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5TypeInferencer typeInferencer;
        public TES5VariableAssignationConversionFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory, TES5ValueFactory valueFactory, TES5TypeInferencer typeInferencer)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
            this.valueFactory = valueFactory;
            this.typeInferencer = typeInferencer;
        }

        public TES5CodeChunkCollection CreateCodeChunk(TES4VariableAssignation chunk, TES5CodeScope codeScope,  TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5CodeChunkCollection codeChunkCollection = new TES5CodeChunkCollection();
            string referenceName = chunk.Reference.StringValue;
            ITES5Referencer reference = this.referenceFactory.CreateReference(referenceName, globalScope, multipleScriptsScope, codeScope.LocalScope);
            ITES5Value value = this.valueFactory.CreateValue(chunk.Value, codeScope, globalScope, multipleScriptsScope);
            if (reference.TES5Type == TES5BasicType.T_GLOBALVARIABLE)
            { //if the reference is in reality a global variable, we will need to convert it by creating a Reference.SetValue(value); call
                //Object call creation
                TES5ObjectCallArguments objectCallArguments = new TES5ObjectCallArguments() { value };
                TES5ObjectCall objectCall = this.objectCallFactory.CreateObjectCall(reference, "SetValue", objectCallArguments);
                codeChunkCollection.Add(objectCall);
            }
            else
            {
                if (reference.ReferencesTo == null) { throw new NullableException(nameof(reference.ReferencesTo)); }
                if (!reference.ReferencesTo.TES5Type.IsPrimitive && value.TES5Type.IsPrimitive)
                {
                    //Hacky!
                    TES5IntegerOrFloat? valueNumber = value as TES5IntegerOrFloat;
                    if (valueNumber != null && valueNumber.ConvertedIntValue == 0)
                    {
                        value = new TES5None();
                    }
                }

                TES5VariableAssignation assignation = TES5VariableAssignationFactory.CreateAssignation(reference, value);
                this.typeInferencer.InferenceObjectByAssignation(reference, value);
                codeChunkCollection.Add(assignation);
                //post analysis.
                //Todo - rethink the prefix here
                ITES5Referencer? referencerValue = value as ITES5Referencer;
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
                    TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(reference, TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL, minusOne);
                    TES5VariableAssignation reassignation = TES5VariableAssignationFactory.CreateAssignation(referencerValue, minusOne);
                    TES5Branch branch = TES5BranchFactory.CreateSimpleBranch(expression, codeScope.LocalScope);
                    branch.MainBranch.CodeScope.AddChunk(reassignation);
                    codeChunkCollection.Add(branch);
                }
            }

            return codeChunkCollection;
        }
    }
}