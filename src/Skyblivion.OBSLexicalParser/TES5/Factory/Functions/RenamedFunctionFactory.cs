using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    /*
     * Class RenamedFunctionFactory
     * Converts functions which are a simple "just move signature over," but with a rename
     */
    class RenamedFunctionFactory : IFunctionFactory
    {
        private readonly string newFunctionName;
        private readonly SimpleFunctionTranslationFactory simpleFunctionTranslationFactory;
        public RenamedFunctionFactory(string newFunctionName, SimpleFunctionTranslationFactory simpleFunctionTranslationFactory)
        {
            this.newFunctionName = newFunctionName;
            this.simpleFunctionTranslationFactory = simpleFunctionTranslationFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return simpleFunctionTranslationFactory.ConvertFunction(calledOn, function, codeScope, globalScope, multipleScriptsScope, newFunctionName);
        }
    }
}