using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetDispositionFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly LogUnknownFunctionFactory logUnknownFunctionFactory;
        public GetDispositionFactory(TES5ObjectCallFactory objectCallFactory, LogUnknownFunctionFactory logUnknownFunctionFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.logUnknownFunctionFactory = logUnknownFunctionFactory;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            var functionArguments = function.Arguments;
            if (TES5PlayerReference.EqualsPlayer(functionArguments[0].StringValue))
            {
                var arguments = new TES5ObjectCallArguments() { calledOn };
                return this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Create(TES5BasicType.T_SKYBDISPOSITIONUTIL), "GetDisposition", arguments);
            }
            return logUnknownFunctionFactory.CreateLogCall(calledOn, function, globalScope);
        }
    }
}