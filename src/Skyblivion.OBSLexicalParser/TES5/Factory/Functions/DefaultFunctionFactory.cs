using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    /*
     * Class DefaultFunctionFactory
     * Converts functions which are a simple ,,just move signature over", as in - we don"t do any changes to the function call
     */
    class DefaultFunctionFactory : IFunctionFactory
    {
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public DefaultFunctionFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            string functionName = function.getFunctionCall().getFunctionName();
            TES4FunctionArguments functionArguments = function.getArguments();
            TES5ObjectCallArguments newArguments = this.objectCallArgumentsFactory.createArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope);
            if (functionName.ToLower().Contains("placeatme"))
            {
                if (newArguments.Any(a => a.Output.Any(b=>b.Contains("TG03LlathasasBust"))))
                {

                }
            }
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, multipleScriptsScope, newArguments);
        }
    }
}