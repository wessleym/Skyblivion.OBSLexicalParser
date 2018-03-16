using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ActivateFactory : IFunctionFactory
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
        public ActivateFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            string functionName = function.getFunctionCall().getFunctionName();
            TES4FunctionArguments functionArguments = function.getArguments();
            if (functionArguments == null || !functionArguments.Any())
            {
                TES5ObjectCallArguments constantArgumentForNoFunctionArguments = new TES5ObjectCallArguments();
                TES5LocalVariable meaningVariable = codeScope.findVariableWithMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
                if (meaningVariable != null)
                {
                    constantArgumentForNoFunctionArguments.add(this.referenceFactory.createReferenceToVariable(meaningVariable));
                }
                else
                {
                    constantArgumentForNoFunctionArguments.add(this.referenceFactory.createReferenceToPlayer());
                }

                constantArgumentForNoFunctionArguments.add(new TES5Bool(true)); //Since default in oblivion is ,,skip the OnActivateBlock", this defaults to ,,abDefaultProcessingOnly = true" in Skyrim
                return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, constantArgumentForNoFunctionArguments);
            }

            TES5ObjectCallArguments constantArgument = new TES5ObjectCallArguments();
            constantArgument.add(this.valueFactory.createValue(functionArguments.getValue(0), codeScope, globalScope, multipleScriptsScope));
            ITES4Value blockOnActivate = functionArguments.getValue(1);
            if (blockOnActivate != null)
            {
                bool blockOnActivateVal = !(bool)blockOnActivate.getData();
                constantArgument.add(new TES5Bool(blockOnActivateVal));
            }

            return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, constantArgument);
        }
    }
}