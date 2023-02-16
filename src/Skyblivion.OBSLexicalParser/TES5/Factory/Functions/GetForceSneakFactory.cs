using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetForceSneakFactory : IFunctionFactory//WTM:  Change:  Added
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetForceSneakFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //WTM:  Note:  @INCONSISTENCE:  This is supposed to check if the actor is being forced to sneak.
            //Since Papyrus can't do that, I'm instead checking if the actor is sneaking at all, force or not.
            return objectCallFactory.CreateObjectCall(calledOn, "IsSneaking", comment: function.Comment);
        }
    }
}
