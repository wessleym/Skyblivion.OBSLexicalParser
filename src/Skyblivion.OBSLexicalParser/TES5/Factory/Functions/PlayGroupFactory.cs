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
    class PlayGroupFactory : IFunctionFactory
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
        public PlayGroupFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            string functionName = "playGamebryoAnimation";
            //this function does not use strings for the names, so i cant really understand what is it.
            //todo refactor
            TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
            ITES4StringValue firstArg = functionArguments.getValue(0);
            convertedArguments.add(new TES5String(firstArg.StringValue));
            /*
            secondArg = functionArguments.getValue(1);
    
        if (secondArg && secondArg.getData() != 0) {
                convertedArguments.add(new TES5Integer(1));
            }*/
            convertedArguments.add(new TES5Bool(true));
            return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
        }
    }
}