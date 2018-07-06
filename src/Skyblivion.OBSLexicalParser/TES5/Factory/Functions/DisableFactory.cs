using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class DisableFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public DisableFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            const string functionName = "Disable";
            TES4FunctionArguments functionArguments = function.Arguments;
            TES5ObjectCallArguments newArguments = new TES5ObjectCallArguments();
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, multipleScriptsScope, newArguments);
        }
    }
}