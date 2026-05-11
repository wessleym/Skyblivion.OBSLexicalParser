using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetActorValueOrBaseActorValueFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly string functionName;
        public GetActorValueOrBaseActorValueFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory, string functionName)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
            this.functionName = functionName;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            //@TODO - This should be fixed on expression-parsing level, with agression and confidence checks adjusted accordingly. There are no retail uses, so im not doing this for now ;)
            ITES4ValueString firstArg = functionArguments[0];
            string firstArgString = firstArg.StringValue;
            if (TES5ReferenceFactory.IsGlobalVariableActorValueName(firstArgString))
            {
                if (!TES5PlayerReference.EqualsPlayer(calledOn))
                {
                    if (calledOn.TES5Type.NativeType == TES5BasicType.T_ACTOR)//WTM:  Change:  I added this if branch.
                    {
                        return this.objectCallFactory.CreateObjectCall(calledOn, functionName, new TES5ObjectCallArguments() { new TES5String(firstArgString) }, comment: function.Comment);
                    }
                    throw new ConversionException(nameof(GetActorValueFactory) + ":  Cannot get attributes on non-player.  Name:  " + calledOn.Name + ", Argument:  " + firstArgString);
                }

                /*
                *  Switch out callee with the reference to attr
                */
                string tes4AttrFirstArg = TES5ReferenceFactory.GetTES4AttrPlusName(firstArgString);
                return this.referenceFactory.CreateReadReference(tes4AttrFirstArg, globalScope, multipleScriptsScope, localScope);
            }
            string argumentString = ActorValueMap.GetRevisedName(firstArgString);
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, new TES5ObjectCallArguments() { new TES5String(argumentString) }, comment: function.Comment);
        }
    }
}