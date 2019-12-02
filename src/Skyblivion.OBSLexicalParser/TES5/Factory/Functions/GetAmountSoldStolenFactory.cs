using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetAmountSoldStolenFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5StaticReferenceFactory staticReferenceFactory;
        public GetAmountSoldStolenFactory(TES5ObjectCallFactory objectCallFactory, TES5StaticReferenceFactory staticReferenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.staticReferenceFactory = staticReferenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return this.objectCallFactory.CreateObjectCall(staticReferenceFactory.Game, "GetAmountSoldStolen");
        }
    }
}