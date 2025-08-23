using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class RemoveMeFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly LogUnknownFunctionFactory logUnknownFunctionFactory;
        public RemoveMeFactory(TES5ObjectCallFactory objectCallFactory, LogUnknownFunctionFactory logUnknownFunctionFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.logUnknownFunctionFactory = logUnknownFunctionFactory;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            if (function.Function.Arguments.Count > 0)
            {
                throw new ConversionException("Multiple arguments of " + function.FunctionCall.FunctionName + " are not supported.");
            }
            if (codeScope.LocalScope.FunctionScope.BlockName == "OnContainerChanged")
            {
                var akNewContainer = codeScope.LocalScope.FunctionScope.GetParameter("akNewContainer");
                var akNewContainerReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(akNewContainer);
                return this.objectCallFactory.CreateObjectCall(akNewContainerReference, "RemoveItem", new TES5ObjectCallArguments() { calledOn });
            }
            return logUnknownFunctionFactory.CreateLogCall(calledOn, function, globalScope, reason: "The container was unknown.");
        }
    }
}