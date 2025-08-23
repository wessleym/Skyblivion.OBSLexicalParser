using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class IsInFactionAndGetCrimeGoldFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly bool violent;
        public IsInFactionAndGetCrimeGoldFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory, bool violent)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
            this.violent = violent;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            if (function.Comment != null)
            {
                throw new ConversionException(function.FunctionCall.FunctionName + "'s comment could not be retained.");
            }
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES5Referencer factionReference = this.referenceFactory.CreateReadReference(functionArguments[0].StringValue, globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { factionReference };
            TES5ObjectCall isInFaction = this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToPlayer(globalScope), "IsInFaction", arguments);
            TES5ComparisonExpression leftExpression = TES5ExpressionFactory.CreateTrueBooleanExpression(isInFaction);
            TES5ObjectCall crimeGold = this.objectCallFactory.CreateObjectCall(factionReference, violent ? "GetCrimeGoldViolent" : "GetCrimeGoldNonViolent");
            TES5ComparisonExpression rightExpression = TES5ExpressionFactory.CreateComparisonExpression(crimeGold, TES5ComparisonExpressionOperator.OPERATOR_GREATER, new TES5Integer(0));
            TES5LogicalExpression logicalExpression = TES5ExpressionFactory.CreateLogicalExpression(leftExpression, TES5LogicalExpressionOperator.OPERATOR_AND, rightExpression);
            return logicalExpression;
        }
    }
}