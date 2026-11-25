using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetOrModFactionReactionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SetOrModFactionReactionFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5CodeChunk ConvertFunction(TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, bool ally, bool selfIsFriendToOtherAndOtherIsFriendToSelf)
        {
            TES4FunctionArguments oldArguments = function.Arguments;
            TES5LocalScope localScope = codeScope.LocalScope;
            ITES5Referencer newCalledOn = this.referenceFactory.CreateReadReference(oldArguments[0].StringValue, globalScope, multipleScriptsScope, localScope);
            ITES5Referencer otherFaction = this.referenceFactory.CreateReadReference(oldArguments[1].StringValue, globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { otherFaction };
            if (selfIsFriendToOtherAndOtherIsFriendToSelf) { arguments.AddRange([new TES5Bool(true), new TES5Bool(true)]); }
            return this.objectCallFactory.CreateObjectCall(newCalledOn, ally ? "SetAlly": "SetEnemy", arguments, comment: function.Comment);
        }
    }
}
