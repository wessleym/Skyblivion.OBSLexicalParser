using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
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
        private TES5ReferenceFactory referenceFactory;
        private TES5ExpressionFactory expressionFactory;
        private TES5VariableAssignationFactory assignationFactory;
        private ESMAnalyzer analyzer;
        private TES5ObjectPropertyFactory objectPropertyFactory;
        private TES5PrimitiveValueFactory primitiveValueFactory;
        private TES5TypeInferencer typeInferencer;
        private MetadataLogService metadataLogService;
        private TES5ValueFactory valueFactory;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public GetPCFactionAttackFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.expressionFactory = expressionFactory;
            this.analyzer = analyzer;
            this.assignationFactory = assignationFactory;
            this.objectPropertyFactory = objectPropertyFactory;
            this.primitiveValueFactory = primitiveValueFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5CodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.getLocalScope();
            TES4FunctionArguments functionArguments = function.getArguments();
            //WARNING: This is not an exact implementation
            //According to cs.elderscrolls.com, its about being in the faction AND having an attack on them ( violent crime )
            //It"s similar but groups all violent wrongdoings ( including assaults, murders etc ).
            ITES5Referencer factionReference = this.referenceFactory.createReadReference((string)functionArguments.getValue(0).getData(), globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
            arguments.add(factionReference);
            TES5ObjectCall isInFaction = this.objectCallFactory.createObjectCall(this.referenceFactory.createReferenceToPlayer(), "IsInFaction", multipleScriptsScope, arguments);
            TES5TrueBooleanExpression leftExpression = this.expressionFactory.createTrueBooleanExpression(isInFaction);
            TES5ObjectCall crimeGoldViolent = this.objectCallFactory.createObjectCall(factionReference, "GetCrimeGoldViolent", multipleScriptsScope);
            TES5ArithmeticExpression right_expression = this.expressionFactory.createArithmeticExpression(crimeGoldViolent, TES5ArithmeticExpressionOperator.OPERATOR_GREATER, new TES5Integer(0));
            TES5LogicalExpression logicalExpression = this.expressionFactory.createLogicalExpression(leftExpression, TES5LogicalExpressionOperator.OPERATOR_AND, right_expression);
            return logicalExpression;
        }
    }
}