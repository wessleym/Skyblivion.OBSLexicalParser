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
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetActorValueFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetActorValueFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            const string functionName = "GetActorValue";
            TES4FunctionArguments functionArguments = function.Arguments;
            //@TODO - This should be fixed on expression-parsing level, with agression and confidence checks adjusted accordingly. There are no retail uses, so im not doing this for now ;)
            Dictionary<string, string> actorValueMap = ActorValueMap.Map;
            ITES4StringValue firstArg = functionArguments[0];
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
                        if (!TES5PlayerReference.EqualsPlayer(calledOn.Name))
                        {
                            if (calledOn.TES5Type.NativeType == TES5BasicType.T_ACTOR)//WTM:  Change:  I added this if branch.
                            {
                                TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments() { new TES5String(firstArgString) };
                                return this.objectCallFactory.CreateObjectCall(calledOn, functionName, convertedArguments);
                            }
                            //We can"t convert those.. and shouldn"t be any, too.
                            throw new ConversionException(nameof(GetActorValueFactory)+":  Cannot get attributes on non-player.  Name:  " + calledOn.Name + ", Argument:  " + firstArgString);
                        }

                        /*
                         *  Switch out callee with the reference to attr
                         */
                        string tes4AttrFirstArg = TES5ReferenceFactory.GetTES4AttrPlusName(firstArgStringLower);
                        return this.referenceFactory.CreateReadReference(tes4AttrFirstArg, globalScope, multipleScriptsScope, localScope);
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
                        TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments() { new TES5String(actorValueMap[firstArgStringLower]) };
                        return this.objectCallFactory.CreateObjectCall(calledOn, functionName, convertedArguments);
                    }

                default:
                    {
                        TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments() { new TES5String(firstArgString) };
                        return this.objectCallFactory.CreateObjectCall(calledOn, functionName, convertedArguments);
                    }
            }
        }
    }
}