using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class IsActionRefFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        public IsActionRefFactory(TES5ReferenceFactory referenceFactory)
        {
            this.referenceFactory = referenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5SignatureParameter? activatorVariable = codeScope.TryGetVariableWithMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
            if (activatorVariable == null)
            {
                throw new ConversionException("isActionRef called in a context where action ref should not be present");
            }

            string dataString = functionArguments[0].StringValue;
            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(TES5ReferenceFactory.CreateReferenceToVariableOrProperty(activatorVariable), TES5ComparisonExpressionOperator.OPERATOR_EQUAL, this.referenceFactory.CreateReadReference(dataString, globalScope, multipleScriptsScope, localScope));
            return expression;
        }
    }
}