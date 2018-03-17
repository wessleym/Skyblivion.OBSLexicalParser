using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    /*
     * Class PopCalledRenameFunctionFactory
     * Pops the first argument to calledOn and renames the function
     * @package Ormin\OBSLexicalParser\TES5\Factory\Functions
     */
    class PopCalledRenameFunctionFactory : IFunctionFactory
    {
        private string newFunctionName;
        private TES5ReferenceFactory referenceFactory;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public PopCalledRenameFunctionFactory(string newFunctionName, TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.newFunctionName = newFunctionName;
            this.referenceFactory = referenceFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5CodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.getLocalScope();
            TES4FunctionArguments functionArguments = function.getArguments();
            ITES5Referencer newCalledOn = this.referenceFactory.createReadReference(functionArguments.popValue(0).StringValue, globalScope, multipleScriptsScope, localScope);
            return this.objectCallFactory.createObjectCall(newCalledOn, this.newFunctionName, multipleScriptsScope, this.objectCallArgumentsFactory.createArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope));
        }
    }
}