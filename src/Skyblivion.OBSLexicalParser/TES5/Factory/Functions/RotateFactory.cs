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
    class RotateFactory : IFunctionFactory
    {
        private TES5ReferenceFactory referenceFactory;
        private TES5VariableAssignationFactory assignationFactory;
        private ESMAnalyzer analyzer;
        private TES5ObjectPropertyFactory objectPropertyFactory;
        private TES5TypeInferencer typeInferencer;
        private MetadataLogService metadataLogService;
        private TES5ValueFactory valueFactory;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public RotateFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
            this.assignationFactory = assignationFactory;
            this.objectPropertyFactory = objectPropertyFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5LocalScope localScope = codeScope.LocalScope;
            int x = 0, y = 0, z = 0;
            int secondArgumentData = (int)functionArguments[1].Data;
            switch ((functionArguments[0].StringValue).ToLower())
            {
                case "x":
                    {
                        x = secondArgumentData;
                        break;
                    }
                case "y":
                    {
                        y = secondArgumentData;
                        break;
                    }
                case "z":
                    {
                        z = secondArgumentData;
                        break;
                    }
            }

            TES5ObjectCallArguments rotateArguments = new TES5ObjectCallArguments();
            rotateArguments.Add(calledOn);
            rotateArguments.Add(new TES5Integer(x));
            rotateArguments.Add(new TES5Integer(y));
            rotateArguments.Add(new TES5Integer(z));
            TES5ObjectCall newFunction = this.objectCallFactory.CreateObjectCall(this.referenceFactory.createReadReference("tTimer", globalScope, multipleScriptsScope, localScope), "Rotate", multipleScriptsScope, rotateArguments);
            return newFunction;
        }
    }
}