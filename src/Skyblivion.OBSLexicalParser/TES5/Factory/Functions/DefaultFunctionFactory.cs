using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    /*
     * Class DefaultFunctionFactory
     * Converts functions which are a simple "just move signature over," as in we don't do any changes to the function call
     */
    class DefaultFunctionFactory : IFunctionFactory
    {
        private readonly SimpleFunctionTranslationFactory simpleFunctionTranslationFactory;
        public DefaultFunctionFactory(SimpleFunctionTranslationFactory simpleFunctionTranslationFactory)
        {
            this.simpleFunctionTranslationFactory = simpleFunctionTranslationFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            string oldAndNewFunctionName = function.FunctionCall.FunctionName;
            return simpleFunctionTranslationFactory.ConvertFunction(calledOn, function, codeScope, globalScope, multipleScriptsScope, oldAndNewFunctionName);
        }
    }
}