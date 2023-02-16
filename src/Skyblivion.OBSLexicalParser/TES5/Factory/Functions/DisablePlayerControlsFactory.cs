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
        public DisablePlayerControlsFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
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
            return this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Game, function, newArgs);
        }
    }
}