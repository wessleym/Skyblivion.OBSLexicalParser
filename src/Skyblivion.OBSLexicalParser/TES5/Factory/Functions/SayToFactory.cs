using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SayToFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly ESMAnalyzer analyzer;
        private readonly TES5ObjectPropertyFactory objectPropertyFactory;
        private readonly TES5TypeInferencer typeInferencer;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public SayToFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5LocalScope localScope = codeScope.LocalScope;
            //Simple implementation without looking.
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
            //if (calledOn.TES5Type != TES5BasicType.T_OBJECTREFERENCE)
            //{
            //    TES5Castable calledOnCastable = calledOn as TES5Reference;
            //    if (calledOn != null && TES5InheritanceGraphAnalyzer.IsExtending(TES5BasicType.T_ACTOR, calledOn.TES5Type))
            //    {
            //        calledOnCastable.ManualCastTo = TES5BasicType.T_OBJECTREFERENCE;
            //    }
            //}
            //arguments.Add(calledOn);
            arguments.Add(this.valueFactory.CreateValue(functionArguments[0], codeScope, globalScope, multipleScriptsScope));
            ITES5Value argument1 = this.valueFactory.CreateValue(functionArguments[1], codeScope, globalScope, multipleScriptsScope);
            if (argument1.TES5Type != TES5BasicType.T_TOPIC)
            {
                TES5Castable argument1Castable = argument1 as TES5Reference;
                if (argument1Castable != null && TES5InheritanceGraphAnalyzer.IsExtending(TES5BasicType.T_TOPIC, argument1.TES5Type))
                {
                    argument1Castable.ManualCastTo = TES5BasicType.T_TOPIC;
                }
            }
            arguments.Add(argument1);
            arguments.Add(new TES5Bool(true));

            //ITES5Referencer timerReference = this.referenceFactory.CreateTimerReadReference(globalScope, multipleScriptsScope, localScope);
            return this.objectCallFactory.CreateObjectCall(calledOn, "LegacySayTo", multipleScriptsScope, arguments);
        }
    }
}