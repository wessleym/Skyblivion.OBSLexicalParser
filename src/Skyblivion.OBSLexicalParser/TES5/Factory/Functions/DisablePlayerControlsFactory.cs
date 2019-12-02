using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class DisablePlayerControlsFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5StaticReferenceFactory staticReferenceFactory;
        public DisablePlayerControlsFactory(TES5ObjectCallFactory objectCallFactory, TES5StaticReferenceFactory staticReferenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.staticReferenceFactory = staticReferenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            string functionName = function.FunctionCall.FunctionName;
            /* Emulating just the same disable player control as in Oblivion */
            TES5ObjectCallArguments newArgs = new TES5ObjectCallArguments()
            {
                new TES5Bool(true),
                new TES5Bool(true),
                new TES5Bool(false),
                new TES5Bool(false),
                new TES5Bool(false),
                new TES5Bool(true),
                new TES5Bool(true),
                new TES5Bool(true)
            };
            return this.objectCallFactory.CreateObjectCall(staticReferenceFactory.Game, functionName, newArgs);
        }
    }
}