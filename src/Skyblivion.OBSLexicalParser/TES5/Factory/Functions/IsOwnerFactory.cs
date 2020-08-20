using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class IsOwnerFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        public IsOwnerFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            string arg0String = functionArguments[0].StringValue;
            ITES5Referencer targetReference = this.referenceFactory.CreateReadReference(arg0String, globalScope, multipleScriptsScope, localScope);
            var arg0Type = targetReference.TES5Type;
            string functionName;
            ITES5Referencer baseReference;
            if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(arg0Type, TES5BasicType.T_ACTORBASE))
            {
                functionName = "GetActorOwner";
                baseReference = targetReference;
            }
            else if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(arg0Type, TES5BasicType.T_ACTOR))
            {
                functionName = "GetActorOwner";
                baseReference = this.objectCallFactory.CreateGetActorBase(targetReference);
            }
            else if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(arg0Type, TES5BasicType.T_FACTION))
            {
                functionName = "GetFactionOwner";
                baseReference = targetReference;
            }
            else
            {
                throw new ConversionException(function.FunctionCall.FunctionName + " should be called with either an ActorBase or a Faction.");
            }
            TES5ObjectCall owner = this.objectCallFactory.CreateObjectCall(calledOn, functionName);
            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(owner, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, baseReference);
            return expression;
        }
    }
}