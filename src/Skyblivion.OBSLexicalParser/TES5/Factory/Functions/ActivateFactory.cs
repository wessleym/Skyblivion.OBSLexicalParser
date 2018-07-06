using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ActivateFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly ESMAnalyzer analyzer;
        private readonly TES5ObjectPropertyFactory objectPropertyFactory;
        private readonly TES5TypeInferencer typeInferencer;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public ActivateFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            string functionName = function.FunctionCall.FunctionName;
            TES4FunctionArguments functionArguments = function.Arguments;
            if (functionArguments == null || !functionArguments.Any())
            {
                TES5ObjectCallArguments constantArgumentForNoFunctionArguments = new TES5ObjectCallArguments();
                TES5LocalVariable meaningVariable = codeScope.GetVariableWithMeaning(TES5LocalVariableParameterMeaning.ACTIVATOR);
                if (meaningVariable != null)
                {
                    constantArgumentForNoFunctionArguments.Add(TES5ReferenceFactory.CreateReferenceToVariable(meaningVariable));
                }
                else
                {
                    constantArgumentForNoFunctionArguments.Add(TES5ReferenceFactory.CreateReferenceToPlayer());
                }

                constantArgumentForNoFunctionArguments.Add(new TES5Bool(true)); //Since default in oblivion is ,,skip the OnActivateBlock", this defaults to ,,abDefaultProcessingOnly = true" in Skyrim
                return this.objectCallFactory.CreateObjectCall(calledOn, functionName, multipleScriptsScope, constantArgumentForNoFunctionArguments);
            }

            TES5ObjectCallArguments constantArgument = new TES5ObjectCallArguments() { this.valueFactory.CreateValue(functionArguments[0], codeScope, globalScope, multipleScriptsScope) };
            ITES4StringValue blockOnActivate = functionArguments.GetOrNull(1);
            if (blockOnActivate != null)
            {
                bool blockOnActivateVal = (int)blockOnActivate.Data== 1;
                constantArgument.Add(new TES5Bool(!blockOnActivateVal));
            }

            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, multipleScriptsScope, constantArgument);
        }
    }
}