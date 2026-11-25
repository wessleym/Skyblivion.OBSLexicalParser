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
    class SetOrForceActorValueFactory : FunctionFactoryBase
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        public SetOrForceActorValueFactory(string tes4FunctionName, string tes5FunctionName, TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
            : base(tes4FunctionName, tes5FunctionName, objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
        }

        public override ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES4ValueString firstArg = functionArguments[0];
            string firstArgString = firstArg.StringValue;
            if (firstArgString.Equals("Speed", StringComparison.OrdinalIgnoreCase))
            {
                ITES4ValueString secondArg = functionArguments[1];
                ITES5Value newSpeedAttribute = this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope);
                return SpeedMultFactory.CreateSetActorValueCodeChunks(calledOn, function, newSpeedAttribute, objectCallFactory);
            }
            if (firstArgString.Equals("Aggression", StringComparison.OrdinalIgnoreCase))
            {
                ITES4ValueString secondArg = functionArguments[1];
                object secondArgObject = secondArg.Constant;
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

                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments
                {
                    new TES5String(firstArgString),
                    convertedArgument2
                };
                return CreateObjectCall(calledOn, TES5FunctionName, convertedArguments, function.Comment);
            }
            if (firstArgString.Equals("Confidence", StringComparison.OrdinalIgnoreCase))
            {
                ITES4ValueString secondArg = functionArguments[1];
                int secondArgData = (int)secondArg.Constant;
                float newValue =
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
                return CreateObjectCall(calledOn, TES5FunctionName, convertedArguments, function.Comment);
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
                return CreateObjectCall(calledOn, TES5FunctionName, convertedArguments, function.Comment);
            }
            if (TES5ReferenceFactory.IsGlobalVariableActorValueName(firstArgString))
            {
                if (!TES5PlayerReference.EqualsPlayer(calledOn))
                {
                    //We can't convert those.. and shouldn't be any, too.
                    throw new ConversionException("[" + TES4FunctionName + "] Cannot set attributes on non-player");
                }

                string tes4AttrFirstArg = TES5ReferenceFactory.GetTES4AttrPlusName(firstArgString);
                ITES5Referencer newCalledOn = this.referenceFactory.CreateReference(tes4AttrFirstArg, globalScope, multipleScriptsScope, localScope);
                ITES4ValueString secondArg = functionArguments[1];
                ITES5Value argument = this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope);
                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments() { argument };
                return CreateObjectCall(newCalledOn, "SetValue", convertedArguments, function.Comment);
            }
            else
            {
                ITES4ValueString secondArg = functionArguments[1];
                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments
                {
                    new TES5String(ActorValueMap.GetRevisedName(firstArgString)),
                    this.valueFactory.CreateValue(secondArg, codeScope, globalScope, multipleScriptsScope)
                };
                return CreateObjectCall(calledOn, TES5FunctionName, convertedArguments, function.Comment);
            }
        }
    }
}