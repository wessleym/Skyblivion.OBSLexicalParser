using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class HasMagicEffectFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly ESMAnalyzer analyzer;
        private readonly TES5ObjectPropertyFactory objectPropertyFactory;
        private readonly TES5TypeInferencer typeInferencer;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public HasMagicEffectFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            string functionName = function.FunctionCall.FunctionName;
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments newArgs = new TES5ObjectCallArguments();
            string dataString = functionArguments[0].StringValue;
            newArgs.Add(this.referenceFactory.CreateReference("Effect"+dataString, globalScope, multipleScriptsScope, localScope));
            /*switch (dataString)
            {

                case "INVI":
                    {
                        newArgs.add(this.referenceFactory.createReference("InvisibillityFFSelf", globalScope, multipleScriptsScope, localScope));
                        break;
                    }

                case "REFA":
                    {
                        newArgs.add(this.referenceFactory.createReference("PerkRestoreStaminaFFSelf", globalScope, multipleScriptsScope, localScope));
                        break;
                    }

                default:
                    {
                        newArgs.add(this.referenceFactory.createReference("Effect" + dataString, globalScope, multipleScriptsScope, localScope));
                        break;
                    }
            }*/
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, newArgs);
        }
    }
}