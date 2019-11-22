using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetPCFactionAttackFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly ESMAnalyzer analyzer;
        private readonly TES5ObjectPropertyFactory objectPropertyFactory;
        private readonly TES5TypeInferencer typeInferencer;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public GetPCFactionAttackFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
            this.objectPropertyFactory = objectPropertyFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            //WARNING: This is not an exact implementation
            //According to cs.elderscrolls.com, its about being in the faction AND having an attack on them ( violent crime )
            //It"s similar but groups all violent wrongdoings ( including assaults, murders etc ).
            ITES5Referencer factionReference = this.referenceFactory.CreateReadReference(functionArguments[0].StringValue, globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { factionReference };
            TES5ObjectCall isInFaction = this.objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToPlayer(globalScope), "IsInFaction", arguments);
            TES5TrueBooleanExpression leftExpression = TES5ExpressionFactory.CreateTrueBooleanExpression(isInFaction);
            TES5ObjectCall crimeGoldViolent = this.objectCallFactory.CreateObjectCall(factionReference, "GetCrimeGoldViolent");
            TES5ComparisonExpression rightExpression = TES5ExpressionFactory.CreateComparisonExpression(crimeGoldViolent, TES5ComparisonExpressionOperator.OPERATOR_GREATER, new TES5Integer(0));
            TES5LogicalExpression logicalExpression = TES5ExpressionFactory.CreateLogicalExpression(leftExpression, TES5LogicalExpressionOperator.OPERATOR_AND, rightExpression);
            return logicalExpression;
        }
    }
}