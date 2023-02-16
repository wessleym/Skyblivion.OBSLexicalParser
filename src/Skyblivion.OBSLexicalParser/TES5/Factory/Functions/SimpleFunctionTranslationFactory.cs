using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SimpleFunctionTranslationFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public SimpleFunctionTranslationFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope, string newFunctionName)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments newArguments = objectCallArgumentsFactory.CreateArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope);
            return this.objectCallFactory.CreateObjectCall(calledOn, newFunctionName, newArguments, function.Comment);
        }
    }
}
