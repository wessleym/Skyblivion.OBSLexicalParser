using Skyblivion.OBSLexicalParser.Builds;
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
                ITES4ValueString secondArg = functionArguments[1];
                int secondArgData = (int)secondArg.Constant;
                int newValue = secondArgData < -80 ? -3 :
                    secondArgData < -50 ? -2 :
                    secondArgData < 0 ? -1 :
                    secondArgData == 0 ? 0 :
                    secondArgData < 50 ? 1 :
                    secondArgData < 80 ? 2 :
                    3;
                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments
                {
                    new TES5String(firstArgString),
                    new TES5Float(newValue)
                };
                return this.objectCallFactory.CreateObjectCall(calledOn, "ModActorValue", convertedArguments, comment: function.Comment);
            }
            if (firstArgString.Equals("Confidence", StringComparison.OrdinalIgnoreCase))
            {
                ITES4ValueString secondArg = functionArguments[1];
                int secondArgData = (int)secondArg.Constant;
                int newValue =
                    secondArgData == -100 ? -4 :
                    secondArgData <= -70 ? -3 :
                    secondArgData <= -30 ? -2 :
                    secondArgData < 0 ? -1 :
                    secondArgData == 0 ? 0 :
                    secondArgData < 30 ? 1 :
                    secondArgData < 70 ? 2 :
                    secondArgData < 99 ? 3 :
                    4;
                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments
                {
                    new TES5String(firstArgString),
                    new TES5Float(newValue)
                };
                return this.objectCallFactory.CreateObjectCall(calledOn, "ModActorValue", convertedArguments, comment: function.Comment);
            }
            if (firstArgString.Equals("Responsibility", StringComparison.OrdinalIgnoreCase))
            {
                ITES4ValueString secondArg = functionArguments[1];
                int secondArgData = (int)secondArg.Constant;
                int newValue =
                    secondArgData == 0 ? 0 :
                    secondArgData < 50 ? 1 :
                    secondArgData < 80 ? 2 :
                    3;
                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments
                {
                    new TES5String("Morality"),
                    new TES5Float(newValue)
                };
                return this.objectCallFactory.CreateObjectCall(calledOn, "ModActorValue", convertedArguments, comment: function.Comment);
            }
            if (firstArgString.Equals("Speed", StringComparison.OrdinalIgnoreCase))
            {
                ITES4ValueString secondArg = functionArguments[1];
                ITES5Value newSpeedAttribute = this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope);
                return SpeedMultFactory.CreateModActorValueCodeChunks(calledOn, function, newSpeedAttribute, objectCallFactory);
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
            else if (TES5PlayerReference.EqualsPlayer(calledOn))
            {
                bool isFame = firstArgString.Contains("Fame", StringComparison.OrdinalIgnoreCase);
                bool isInfamy = firstArgString.Contains("Infamy", StringComparison.OrdinalIgnoreCase);
                if (isFame || isInfamy)
                {
                    ITES4ValueString secondArg = functionArguments[1];
                    ITES5Value argument = this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope);
                    TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments() { argument };
                    return this.objectCallFactory.CreateObjectCall(isFame ? referenceFactory.CreateFameReference(globalScope, multipleScriptsScope, localScope) : referenceFactory.CreateInfamyReference(globalScope, multipleScriptsScope, localScope), "Mod", convertedArguments, comment: function.Comment);
                }
            }
            {
                ITES4ValueString secondArg = functionArguments[1];
                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments
                {
                    new TES5String(ActorValueMap.GetRevisedName(firstArgString)),
                    this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope)
                };
                return this.objectCallFactory.CreateObjectCall(calledOn, "ModActorValue", convertedArguments, comment: function.Comment);
            }
        }
    }
}