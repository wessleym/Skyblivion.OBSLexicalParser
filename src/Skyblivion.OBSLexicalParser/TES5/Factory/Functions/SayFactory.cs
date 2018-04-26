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
    class SayFactory : IFunctionFactory
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
        public SayFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
            arguments.Add(calledOn);
            ITES5Value argument1 = this.valueFactory.createValue(functionArguments[0], codeScope, globalScope, multipleScriptsScope);
            if (argument1.TES5Type != TES5BasicType.T_TOPIC)
            {
                TES5Castable argument1Castable = argument1 as TES5Reference;
                if (argument1Castable != null && TES5InheritanceGraphAnalyzer.isExtending(TES5BasicType.T_TOPIC, argument1.TES5Type))
                {
                    argument1Castable.ManualCastTo = TES5BasicType.T_TOPIC;
                }
            }
            arguments.Add(argument1);
            ITES4StringValue optionalFlag = functionArguments.GetOrNull(2);
            if (optionalFlag != null)
            {
                string optionalFlagDataString = optionalFlag.StringValue;
                if (ESMAnalyzer._instance().getFormTypeByEDID(optionalFlagDataString).Value!= TES4RecordType.REFR.Name)
                {
                    this.metadataLogService.WriteLine("ADD_SPEAK_AS_ACTOR", new string[] { optionalFlagDataString });
                    optionalFlag = new TES4ApiToken(optionalFlag.Data+"Ref");
                }

                arguments.Add(this.valueFactory.createValue(optionalFlag, codeScope, globalScope, multipleScriptsScope));
            }
            else
            {
                arguments.Add(new TES5None());
            }

            arguments.Add(new TES5Bool(true));
            ITES5Referencer timerReference = this.referenceFactory.CreateTimerReadReference(globalScope, multipleScriptsScope, localScope);
            return this.objectCallFactory.CreateObjectCall(timerReference, "LegacySay", multipleScriptsScope, arguments);
        }
    }
}