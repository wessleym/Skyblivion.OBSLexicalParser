using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetRandomPercentFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5StaticReferenceFactory staticReferenceFactory;
        public GetRandomPercentFactory(TES5ObjectCallFactory objectCallFactory, TES5StaticReferenceFactory staticReferenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.staticReferenceFactory = staticReferenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            const string functionName = "RandomInt";
            TES5ObjectCallArguments methodArguments = new TES5ObjectCallArguments()
            {
                new TES5Integer(0),
                new TES5Integer(99)
            };
            return this.objectCallFactory.CreateObjectCall(staticReferenceFactory.Utility, functionName, methodArguments);
        }
    }
}