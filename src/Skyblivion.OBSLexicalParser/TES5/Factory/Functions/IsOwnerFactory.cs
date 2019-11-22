using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class IsOwnerFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly ESMAnalyzer analyzer;
        public IsOwnerFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory, ESMAnalyzer analyzer)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            string dataString = functionArguments[0].StringValue;
            ITES5Referencer targetReference = this.referenceFactory.CreateReadReference(dataString, globalScope, multipleScriptsScope, localScope);
            ITES5Type dataType = this.analyzer.GetFormTypeByEDID(dataString);
            TES5ObjectCall owner;
            ITES5Referencer baseReference;
            if (dataType == TES5BasicType.T_FACTION)
            {
                owner = this.objectCallFactory.CreateObjectCall(calledOn, "GetFactionOwner");
                baseReference = targetReference;
            }
            else
            {
                owner = this.objectCallFactory.CreateObjectCall(calledOn, "GetActorOwner");
                baseReference = this.objectCallFactory.CreateGetActorBase(targetReference);
            }

            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(owner, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, baseReference);
            return expression;
        }
    }
}