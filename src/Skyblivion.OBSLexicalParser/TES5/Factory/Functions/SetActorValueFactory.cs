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
    class SetActorValueFactory : IFunctionFactory
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
        public SetActorValueFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ExpressionFactory expressionFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer, TES5PrimitiveValueFactory primitiveValueFactory, TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            TES4FunctionArguments functionArguments = function.getArguments();
            TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
            Dictionary<string, string> actorValueMap = ActorValueMap.Map;
            ITES4StringValue firstArg = functionArguments.getValue(0);
            string firstArgData = firstArg.StringValue;
            string firstArgDataLower = firstArgData.ToLower();
            switch (firstArgDataLower)
            {
                case "strength":
                case "intelligence":
                case "willpower":
                case "agility":
                case "endurance":
                case "personality":
                case "luck":
                    {
                        if (calledOn.getName() != "player")
                        {
                            //We can"t convert those.. and shouldn"t be any, too.
                            throw new ConversionException("[SetAV] Cannot set attributes on non-player");
                        }

                        string functionName = "SetValue";
                        calledOn = this.referenceFactory.createReference("TES4Attr"+PHPFunction.UCWords(firstArgDataLower), globalScope, multipleScriptsScope, localScope);
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        convertedArguments.add(this.valueFactory.createValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
                    }

                case "speed":
                    {
                        string functionName = "ForceMovementSpeed";
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        convertedArguments.add(this.valueFactory.createValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
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
                        string functionName = "SetActorValue";
                        convertedArguments.add(new TES5String(actorValueMap[firstArgDataLower]));
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        convertedArguments.add(this.valueFactory.createValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
                    }

                case "aggression":
                    {
                        string functionName = "SetActorValue";
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        int secondArgData = (int)secondArg.getData();
                        int newValue;
                        if (secondArgData == 0)
                        {
                            newValue = 0;
                        }
                        else if (secondArgData > 0 && secondArgData < 50)
                        {
                            newValue = 1;
                        }
                        else if (secondArgData >= 50 && secondArgData < 80)
                        {
                            newValue = 2;
                        }
                        else
                        {
                            newValue = 3;
                        }

                        convertedArguments.add(new TES5String(firstArgData));
                        convertedArguments.add(new TES5Float(newValue));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
                    }

                case "confidence":
                    {
                        string functionName = "SetActorValue";
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        int secondArgData = (int)secondArg.getData();
                        int newValue;
                        if (secondArgData == 0)
                        {
                            newValue = 0;
                        }
                        else if (secondArgData > 0 && secondArgData < 30)
                        {
                            newValue = 1;
                        }
                        else
                        if (secondArgData >= 30 && secondArgData < 70)
                        {
                            newValue = 2;
                        }
                        else
                        if (secondArgData >= 70 && secondArgData < 99)
                        {
                            newValue = 3;
                        }
                        else
                        {
                            newValue = 4;
                        }

                        convertedArguments.add(new TES5String(firstArgData));
                        convertedArguments.add(new TES5Float(newValue));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
                    }

                default:
                    {
                        string functionName = "SetActorValue";
                        convertedArguments.add(new TES5String(firstArgData));
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        convertedArguments.add(this.valueFactory.createValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
                    }
            }
        }
    }
}