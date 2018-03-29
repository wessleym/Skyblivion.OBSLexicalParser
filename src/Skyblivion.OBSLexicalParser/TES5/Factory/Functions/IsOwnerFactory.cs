using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class IsOwnerFactory : IFunctionFactory
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
        public IsOwnerFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.getLocalScope();
            TES4FunctionArguments functionArguments = function.getArguments();
            string dataString = functionArguments.getValue(0).StringValue;
            ITES5Referencer targetReference = this.referenceFactory.createReadReference(dataString, globalScope, multipleScriptsScope, localScope);
            ITES5Type dataType = ESMAnalyzer._instance().getFormTypeByEDID(dataString);
            TES5ObjectCall owner;
            ITES5Referencer baseReference;
            if (dataType == TES5BasicType.T_FACTION)
            {
                owner = this.objectCallFactory.createObjectCall(calledOn, "GetFactionOwner", multipleScriptsScope);
                baseReference = targetReference;
            }
            else
            {
                owner = this.objectCallFactory.createObjectCall(calledOn, "GetActorOwner", multipleScriptsScope);
                baseReference = this.objectCallFactory.createObjectCall(targetReference, "GetActorBase", multipleScriptsScope);
            }

            TES5ArithmeticExpression expression = this.expressionFactory.createArithmeticExpression(owner, TES5ArithmeticExpressionOperator.OPERATOR_EQUAL, baseReference);
            return expression;
        }
    }
}