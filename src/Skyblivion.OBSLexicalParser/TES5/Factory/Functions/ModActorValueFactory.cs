using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ModActorValueFactory : IFunctionFactory
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
        public ModActorValueFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
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
            TES4FunctionArguments functionArguments = function.getArguments();
            TES5LocalScope localScope = codeScope.getLocalScope();
            if (function.getFunctionCall().getFunctionName().Equals("modpcskill", StringComparison.OrdinalIgnoreCase))
            {
                /*
                 * MODPCSkill means we will need to call upon the player object
                 */
                calledOn = this.referenceFactory.createReferenceToPlayer();
            }

            TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
            var actorValueMap = ActorValueMap.Map;
            string firstArgData = functionArguments.getValue(0).StringValue;
            string firstArgDataLower = firstArgData.ToLower();
            switch (firstArgDataLower)
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
                            throw new ConversionException("[ModAV] Cannot set attributes on non-player");
                        }

                        string functionName = "SetValue";
                        string firstArgDataLowerUCWords = PHPFunction.UCWords(firstArgDataLower);
                        string tes4AttrFirstArg = "TES4Attr" + firstArgDataLowerUCWords;
                        /*
                         *  Switch out callee with the reference to attr
                         */
                        ITES5Referencer newCalledOn = this.referenceFactory.createReference(tes4AttrFirstArg, globalScope, multipleScriptsScope, localScope);
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        ITES5Value addedValue = this.valueFactory.createValue(secondArg, codeScope, globalScope, multipleScriptsScope);
                        convertedArguments.add(TES5ExpressionFactory.createBinaryExpression(addedValue, TES5BinaryExpressionOperator.OPERATOR_ADD, this.referenceFactory.createReadReference(tes4AttrFirstArg, globalScope, multipleScriptsScope, localScope)));
                        return this.objectCallFactory.createObjectCall(newCalledOn, functionName, multipleScriptsScope, convertedArguments);
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
                        string functionName = "ModActorValue";
                        convertedArguments.add(new TES5String(actorValueMap[firstArgDataLower]));
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        convertedArguments.add(this.valueFactory.createValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
                    }

                case "aggression":
                    {
                        string functionName = "ModActorValue";
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        int secondArgData = (int)secondArg.getData();
                        int newValue;
                        if (secondArgData < -80)
                        {
                            newValue = -3;
                        }
                        else if (secondArgData >= -80 && secondArgData < -50)
                        {
                            newValue = -2;
                        }
                        else if (secondArgData >= -50 && secondArgData < 0)
                        {
                            newValue = -1;
                        }
                        else
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
                        string functionName = "ModActorValue";
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        int secondArgData = (int)secondArg.getData();
                        int newValue;
                        if (secondArgData == -100)
                        {
                            newValue = -4;
                        }
                        else if (secondArgData <= -70 && secondArgData > -100)
                        {
                            newValue = -3;
                        }
                        else
                        if (secondArgData <= -30 && secondArgData > -70)
                        {
                            newValue = -2;
                        }
                        else
                        if (secondArgData < 0 && secondArgData > -30)
                        {
                            newValue = -1;
                        }
                        else
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
                        string functionName = "ModActorValue";
                        convertedArguments.add(new TES5String(firstArgData));
                        ITES4StringValue secondArg = functionArguments.getValue(1);
                        convertedArguments.add(this.valueFactory.createValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return this.objectCallFactory.createObjectCall(calledOn, functionName, multipleScriptsScope, convertedArguments);
                    }
            }
        }
    }
}