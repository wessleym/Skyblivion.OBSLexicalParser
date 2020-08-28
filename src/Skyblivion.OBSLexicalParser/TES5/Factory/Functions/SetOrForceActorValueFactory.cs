using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Other;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetOrForceActorValueFactory : FunctionFactoryBase
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ValueFactory valueFactory;
        public SetOrForceActorValueFactory(string tes4FunctionName, string tes5FunctionName, TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
            : base(tes4FunctionName, tes5FunctionName, objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
        }

        public override ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
            ITES4StringValue firstArg = functionArguments[0];
            string firstArgString = firstArg.StringValue;
            string firstArgStringLower = firstArgString.ToLower();
            switch (firstArgStringLower)
            {
                case "strength":
                case "intelligence":
                case "willpower":
                case "agility":
                case "endurance":
                case "personality":
                case "luck":
                    {
                        if (!TES5PlayerReference.EqualsPlayer(calledOn.Name))
                        {
                            //We can"t convert those.. and shouldn"t be any, too.
                            throw new ConversionException("[" + TES4FunctionName + "] Cannot set attributes on non-player");
                        }

                        const string functionName = "SetValue";
                        string tes4AttrFirstArg = TES5ReferenceFactory.GetTES4AttrPlusName(firstArgStringLower);
                        calledOn = this.referenceFactory.CreateReference(tes4AttrFirstArg, globalScope, multipleScriptsScope, localScope);
                        ITES4StringValue secondArg = functionArguments[1];
                        convertedArguments.Add(this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return CreateObjectCall(calledOn, functionName, convertedArguments);
                    }

                case "speed":
                    {
                        const string functionName = "ForceMovementSpeed";
                        ITES4StringValue secondArg = functionArguments[1];
                        convertedArguments.Add(this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return CreateObjectCall(calledOn, functionName, convertedArguments);
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
                        convertedArguments.Add(new TES5String(ActorValueMap.Map[firstArgStringLower]));
                        ITES4StringValue secondArg = functionArguments[1];
                        convertedArguments.Add(this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return CreateObjectCall(calledOn, TES5FunctionName, convertedArguments);
                    }

                case "aggression":
                    {
                        ITES4StringValue secondArg = functionArguments[1];
                        object secondArgObject = secondArg.Data;
                        Nullable<int> secondArgNullableInt = secondArgObject as Nullable<int>;
                        AST.Value.ITES5Value convertedArgument2;
                        if (secondArgNullableInt != null)
                        {//WTM:  Change:  This apparently went unnoticed in the PHP version.  The second argument is not always an integer.  Sometimes its a variable reference.
                            //But the value of the variable will not be properly scaled to the values below, so the converted script may operate incorrectly.
                            int secondArgInt = secondArgNullableInt.Value;
                            float newValue;
                            if (secondArgInt == 0)
                            {
                                newValue = 0;
                            }
                            else if (secondArgInt > 0 && secondArgInt < 50)
                            {
                                newValue = 1;
                            }
                            else if (secondArgInt >= 50 && secondArgInt < 80)
                            {
                                newValue = 2;
                            }
                            else
                            {
                                newValue = 3;
                            }
                            convertedArgument2 = new TES5Float(newValue);
                        }
                        else
                        {
                            convertedArgument2 = this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope);
                        }

                        convertedArguments.Add(new TES5String(firstArgString));
                        convertedArguments.Add(convertedArgument2);
                        return CreateObjectCall(calledOn, TES5FunctionName, convertedArguments);
                    }

                case "confidence":
                    {
                        ITES4StringValue secondArg = functionArguments[1];
                        int secondArgData = (int)secondArg.Data;
                        float newValue;
                        if (secondArgData == 0)
                        {
                            newValue = 0;
                        }
                        else if (secondArgData > 0 && secondArgData < 30)
                        {
                            newValue = 1;
                        }
                        else if (secondArgData >= 30 && secondArgData < 70)
                        {
                            newValue = 2;
                        }
                        else if (secondArgData >= 70 && secondArgData < 99)
                        {
                            newValue = 3;
                        }
                        else
                        {
                            newValue = 4;
                        }

                        convertedArguments.Add(new TES5String(firstArgString));
                        convertedArguments.Add(new TES5Float(newValue));
                        return CreateObjectCall(calledOn, TES5FunctionName, convertedArguments);
                    }

                default:
                    {
                        convertedArguments.Add(new TES5String(firstArgString));
                        ITES4StringValue secondArg = functionArguments[1];
                        convertedArguments.Add(this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                        return CreateObjectCall(calledOn, TES5FunctionName, convertedArguments);
                    }
            }
        }
    }
}