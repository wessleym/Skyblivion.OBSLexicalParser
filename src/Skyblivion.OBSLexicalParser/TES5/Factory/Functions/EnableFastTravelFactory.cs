using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class EnableFastTravelFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public EnableFastTravelFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            string functionName = function.FunctionCall.FunctionName;
            TES4FunctionArguments functionArguments = function.Arguments;
            //@INCONSISTENCE @TODO: Entering an interior Cell and then exiting to an exterior will reset Fast Travel to the enabled state.
            return this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Game, functionName, this.objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
        }
    }
}