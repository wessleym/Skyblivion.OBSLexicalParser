using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SayFactory : IFunctionFactory
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
        public SayFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            TES4FunctionArguments functionArguments = function.getArguments();
            TES5LocalScope localScope = codeScope.getLocalScope();
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
            arguments.add(calledOn);
            arguments.add(this.valueFactory.createValue(functionArguments.getValue(0), codeScope, globalScope, multipleScriptsScope));
            ITES4StringValue optionalFlag = functionArguments.getValue(2);
            if (optionalFlag != null)
            {
                string optionalFlagDataString = optionalFlag.StringValue;
                if (ESMAnalyzer._instance().getFormTypeByEDID(optionalFlagDataString).value() != TES4RecordType.REFR.Name)
                {
                    this.metadataLogService.add("ADD_SPEAK_AS_ACTOR", new string[] { optionalFlagDataString });
                    optionalFlag = new TES4ApiToken(optionalFlag.getData()+"Ref");
                }

                arguments.add(this.valueFactory.createValue(optionalFlag, codeScope, globalScope, multipleScriptsScope));
            }
            else
            {
                arguments.add(new TES5None());
            }

            arguments.add(new TES5Bool(true));
            ITES5Referencer timerReference = this.referenceFactory.createReadReference("tTimer", globalScope, multipleScriptsScope, localScope);
            return this.objectCallFactory.createObjectCall(timerReference, "LegacySay", multipleScriptsScope, arguments);
        }
    }
}