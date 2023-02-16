using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class WakeUpPCFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public WakeUpPCFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Change:  Apparently moving the player wakes up the player.  I want to see if this works.
            var playerRef = TES5ReferenceFactory.CreateReferenceToPlayer(globalScope);
            return objectCallFactory.CreateObjectCall(playerRef, "MoveTo", new TES5ObjectCallArguments() { playerRef }, comment: function.Comment);
        }
    }
}
