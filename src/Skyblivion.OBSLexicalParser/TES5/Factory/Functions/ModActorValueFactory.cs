using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Other;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ModActorValueFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public ModActorValueFactory(TES5ReferenceFactory referenceFactory, TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5LocalScope localScope = codeScope.LocalScope;
            if (function.FunctionCall.FunctionName.Equals("ModPCSkill", StringComparison.OrdinalIgnoreCase))
            {
                /*
                 * MODPCSkill means we will need to call upon the player object
                 */
                calledOn = TES5ReferenceFactory.CreateReferenceToPlayer(globalScope);
            }

            string firstArgString = functionArguments[0].StringValue;
            if (firstArgString.Equals("Aggression", StringComparison.OrdinalIgnoreCase))
            {
                const string functionName = "ModActorValue";
                ITES4ValueString secondArg = functionArguments[1];
                int secondArgData = (int)secondArg.Constant;
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

                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
                convertedArguments.Add(new TES5String(firstArgString));
                convertedArguments.Add(new TES5Float(newValue));
                return this.objectCallFactory.CreateObjectCall(calledOn, functionName, convertedArguments, comment: function.Comment);
            }
            if (firstArgString.Equals("Confidence", StringComparison.OrdinalIgnoreCase))
            {
                const string functionName = "ModActorValue";
                ITES4ValueString secondArg = functionArguments[1];
                int secondArgData = (int)secondArg.Constant;
                int newValue;
                if (secondArgData == -100)
                {
                    newValue = -4;
                }
                else if (secondArgData <= -70 && secondArgData > -100)
                {
                    newValue = -3;
                }
                else if (secondArgData <= -30 && secondArgData > -70)
                {
                    newValue = -2;
                }
                else if (secondArgData < 0 && secondArgData > -30)
                {
                    newValue = -1;
                }
                else if (secondArgData == 0)
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

                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
                convertedArguments.Add(new TES5String(firstArgString));
                convertedArguments.Add(new TES5Float(newValue));
                return this.objectCallFactory.CreateObjectCall(calledOn, functionName, convertedArguments, comment: function.Comment);
            }
            if (TES5ReferenceFactory.IsGlobalVariableActorValueName(firstArgString))
            {
                if (!TES5PlayerReference.EqualsPlayer(calledOn))
                {
                    //We can't convert those.. and shouldn't be any, too.
                    throw new ConversionException(nameof(ModActorValueFactory) + ":  Cannot set attributes on non-player.  Name:  " + calledOn.Name + ", Argument:  " + firstArgString);
                }

                string tes4AttrFirstArg = TES5ReferenceFactory.GetTES4AttrPlusName(firstArgString);
                ITES5Referencer newCalledOn = this.referenceFactory.CreateReference(tes4AttrFirstArg, globalScope, multipleScriptsScope, localScope);
                ITES4ValueString secondArg = functionArguments[1];
                ITES5Value argument = this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope);
                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments() { argument };
                return this.objectCallFactory.CreateObjectCall(newCalledOn, "Mod", convertedArguments, comment: function.Comment);
            }
            else
            {
                ITES4ValueString secondArg = functionArguments[1];
                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments();
                convertedArguments.Add(new TES5String(ActorValueMap.GetRevisedName(firstArgString)));
                convertedArguments.Add(this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope));
                return this.objectCallFactory.CreateObjectCall(calledOn, "ModActorValue", convertedArguments, comment: function.Comment);
            }
        }
    }
}