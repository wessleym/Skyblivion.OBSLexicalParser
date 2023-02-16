using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetIsIdFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetIsIdFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            if (function.Comment != null)
            {
                throw new ConversionException(function.FunctionCall.FunctionName + "'s comment could not be retained.");
            }
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            //Made in post-analysis
            TES5ObjectCall functionThis = this.objectCallFactory.CreateObjectCall(calledOn, "GetBaseObject");
            ITES5Referencer argument = this.referenceFactory.CreateReadReference(functionArguments[0].StringValue, globalScope, multipleScriptsScope, localScope);
            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(functionThis, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, argument);
            return expression;
        }
    }
}