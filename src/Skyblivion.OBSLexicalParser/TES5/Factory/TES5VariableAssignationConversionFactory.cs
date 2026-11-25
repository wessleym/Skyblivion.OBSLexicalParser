using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.Builds;
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
        public TES5VariableAssignationConversionFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory, TES5ValueFactory valueFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
            this.valueFactory = valueFactory;
        }

        private TES5ObjectCall CreateGlobalVariableSetValueOrMod(TES4VariableAssignation variableAssignation, ITES5Referencer reference, ITES5Value value)
        {
            TES5ArithmeticExpression? arithmeticExpression = value as TES5ArithmeticExpression;
            if (arithmeticExpression != null)
            {
                TES5ObjectCall? leftObjectCall = arithmeticExpression.LeftValue as TES5ObjectCall;
                if (leftObjectCall != null && leftObjectCall.AccessedObject.TES5Type == TES5BasicType.T_GLOBALVARIABLE && reference.Name == leftObjectCall.AccessedObject.Name && leftObjectCall.FunctionName == "GetValue")
                {
                    return this.objectCallFactory.CreateObjectCall(reference, "Mod", new TES5ObjectCallArguments() { arithmeticExpression.RightValue }, comment: variableAssignation.Comment);
                }
            }
            return this.objectCallFactory.CreateObjectCall(reference, "SetValue", new TES5ObjectCallArguments() { value }, comment: variableAssignation.Comment);
        }

        public TES5CodeChunkCollection CreateCodeChunk(TES4VariableAssignation variableAssignation, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5CodeChunkCollection codeChunkCollection = new TES5CodeChunkCollection();
            string referenceName = variableAssignation.Reference.StringValue;
            ITES5Referencer reference = this.referenceFactory.CreateReference(referenceName, globalScope, multipleScriptsScope, codeScope.LocalScope);
            ITES5Value value = this.valueFactory.CreateValue(variableAssignation.Value, codeScope, globalScope, multipleScriptsScope);
            if (reference.TES5Type == TES5BasicType.T_GLOBALVARIABLE)
            {
                codeChunkCollection.Add(CreateGlobalVariableSetValueOrMod(variableAssignation, reference, value));
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

                TES5Comment? comment = variableAssignation.Comment != null ? TES5CommentFactory.Construct(variableAssignation.Comment) : null;
                TES5VariableAssignation assignation = TES5VariableAssignationFactory.CreateAssignation(reference, value, comment: comment);
                TES5TypeInferencer.InferenceObjectByAssignation(reference, value);
                codeChunkCollection.Add(assignation);
                //post analysis.
                //Todo - rethink the prefix here
                ITES5Referencer? referencerValue = value as ITES5Referencer;
                if (referencerValue != null && referencerValue.Name == TES5Property.AddPropertyNameSuffix(TES5ReferenceFactory.MESSAGEBOX_VARIABLE_CONST))
                {
                    /*
                     * Create block:
                     * variable = this.TES4_MESSAGEBOX_RESULT; ; assignation
                     * if(variable != -1) ; branch, expression
                     *   this.TES4_MESSAGEBOX_RESULT = -1; ; reassignation
                     * endIf
                     */
                    TES5Integer negativeOne = new TES5Integer(-1);
                    TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(reference, TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL, negativeOne);
                    TES5VariableAssignation reassignation = TES5VariableAssignationFactory.CreateAssignation(referencerValue, negativeOne);
                    TES5IfElseChain ifElseChain = TES5IfElseFactory.CreateIfElseChain(expression, codeScope.LocalScope);
                    ifElseChain.IfClause.CodeScope.AddChunk(reassignation);
                    codeChunkCollection.Add(ifElseChain);
                }
                if (reference.TES5Type.NativeType == TES5BasicType.T_FLOAT && (value as TES5ObjectCall)?.AccessedObject.Name == TES5Property.AddPropertyNameSuffix("GameDaysPassed"))
                {
                    ((TES5Castable)value).ManualCastTo = TES5BasicType.T_INT;
                }
            }

            return codeChunkCollection;
        }
    }
}