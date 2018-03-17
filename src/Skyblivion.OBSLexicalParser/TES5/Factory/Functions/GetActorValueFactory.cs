using Ormin.OBSLexicalParser.TES5.Factory;
using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetActorValueFactory : IFunctionFactory
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
        public GetActorValueFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            TES5LocalScope localScope = codeScope.getLocalScope();
            string functionName = function.getFunctionCall().getFunctionName();
            TES4FunctionArguments functionArguments = function.getArguments();
            //@TODO - This should be fixed on expression-parsing level, with agression and confidence checks adjusted accordingly. There are no retail uses, so im not doing this for now ;)
            Dictionary<string, string> actorValueMap = ActorValueMap.Map;
            ITES4StringValue firstArg = functionArguments.getValue(0);
            string firstArgString = firstArg.StringValue;
            string firstArgStringLower = firstArgString.ToLower();
            switch (firstArgStringLower)
            {
                case "strength":
                case "intelligence":
                case "willpower":
                case "agility":
                case "speed":
                case "endurance":
                case "personality":
                case "luck":
                    {
                        if (calledOn.getName() != "player")
                        {
                            //We can"t convert those.. and shouldn"t be any, too.
                            throw new ConversionException("[ModAV] Cannot get attributes on non-player");
                        }

                        /*
                         *  Switch out callee with the reference to attr
                         */
                        return this.referenceFactory.createReadReference("TES4Attr" + PHPFunction.UCWords(firstArgStringLower), globalScope, multipleScriptsScope, localScope);
                    }

                case "fatigue":
                case "armorer":
                case "security":
                case "acrobatics":
                case "mercantile":
                case "mysticism": //It doesn"t exist in Skyrim - defaulting to Illusion..
                case "blade":
                case "blunt":
                case "encumbrance":
                case "spellabsorbchance":
                case "resistfire":
                case "resistfrost":
                case "resistdisease":
                case "resistmagic":
                case "resistpoison":
                case "resistshock":
                    {
                        TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
                        convertedArguments.add(new TES5String(actorValueMap[firstArgStringLower]));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
                    }

                default:
                    {
                        TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
                        convertedArguments.add(new TES5String(firstArgString));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
                    }
            }
        }
    }
}