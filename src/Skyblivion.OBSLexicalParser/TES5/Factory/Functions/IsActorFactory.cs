using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class IsActorFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public IsActorFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            /*
            //evaluated in random compile time, cannot assure all inference has happened already.
            TES5Bool boolean = new TES5Bool(TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(calledOn.TES5Type, TES5BasicType.T_ACTOR));
            return boolean;
            */
            //WTM:  Change:  Added:
            TES5ObjectCall actorTypeNPCObjectCall = objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Keyword, "GetKeyword", new TES5ObjectCallArguments() { new TES5String("ActorTypeNPC") });//GetKeyword is an SKSE function
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { actorTypeNPCObjectCall };
            return objectCallFactory.CreateObjectCall(calledOn, "HasKeyword", arguments, comment: function.Comment);
        }
    }
}