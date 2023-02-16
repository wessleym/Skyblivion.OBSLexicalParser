using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SendTrespassAlarmFactory : IFunctionFactory//WTM:  Change:  Added
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public SendTrespassAlarmFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Note:  This always uses the player to send the tresspass alarm.  This might mean the player ultimately sends the trespass alarm for the player.
            //Oblivion doesn't seem to have a sender, only a trespasser.
            TES5ObjectCallArguments arguments = objectCallArgumentsFactory.CreateArgumentList(function.Arguments, codeScope, globalScope, multipleScriptsScope);
            return objectCallFactory.CreateObjectCall(TES5ReferenceFactory.CreateReferenceToPlayer(globalScope), function.FunctionCall.FunctionName, arguments, comment: function.Comment);
        }
    }
}
