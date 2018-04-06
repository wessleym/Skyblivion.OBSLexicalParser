using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    /*
     * Class RenamedFunctionFactory
     * Converts functions which are a simple ,,just move signature over", but with a rename
     */
    class RenamedFunctionFactory : IFunctionFactory
    {
        private string newFunctionName;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public RenamedFunctionFactory(string newFunctionName, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.newFunctionName = newFunctionName;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.getArguments();
            return this.objectCallFactory.CreateObjectCall(calledOn, this.newFunctionName, multipleScriptsScope, this.objectCallArgumentsFactory.createArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
        }
    }
}