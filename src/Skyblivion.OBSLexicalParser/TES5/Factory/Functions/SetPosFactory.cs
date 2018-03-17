using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetPosFactory : IFunctionFactory
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
        public SetPosFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            TES4FunctionArguments functionArguments = function.getArguments();
            TES5ObjectCallArguments callArguments = new TES5ObjectCallArguments();
            TES5ObjectCall dummyX = this.objectCallFactory.createObjectCall(calledOn, "GetPositionX", multipleScriptsScope);
            TES5ObjectCall dummyY = this.objectCallFactory.createObjectCall(calledOn, "GetPositionY", multipleScriptsScope);
            TES5ObjectCall dummyZ = this.objectCallFactory.createObjectCall(calledOn, "GetPositionZ", multipleScriptsScope);
            ITES5Value[] argList;
            switch ((functionArguments.getValue(0).StringValue).ToLower())
            {
                case "x":
                    {
                        argList = new ITES5Value[]
                        {
                            this.valueFactory.createValue(functionArguments.getValue(1), codeScope, globalScope, multipleScriptsScope),
                            dummyY,
                            dummyZ
                        };
                        break;
                    }

                case "y":
                    {
                        argList = new ITES5Value[]
                        {
                            dummyX,
                            this.valueFactory.createValue(functionArguments.getValue(1), codeScope, globalScope, multipleScriptsScope),
                            dummyZ
                        };
                        break;
                    }

                case "z":
                    {
                        argList = new ITES5Value[]
                        {
                            dummyX,
                            dummyY,
                            this.valueFactory.createValue(functionArguments.getValue(1), codeScope, globalScope, multipleScriptsScope)
                        };
                        break;
                    }

                default:
                    {
                        throw new ConversionException("setPos can handle only X,Y,Z parameters.");
                    }
            }

            foreach (var argListC in argList)
            {
                callArguments.add(argListC);
            }

            return this.objectCallFactory.createObjectCall(calledOn, "SetPosition", multipleScriptsScope, callArguments);
        }
    }
}