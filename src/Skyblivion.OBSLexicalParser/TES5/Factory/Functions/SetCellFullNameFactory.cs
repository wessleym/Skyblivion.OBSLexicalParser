using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetCellFullNameFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public SetCellFullNameFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            ITES4ValueString arg0Value;
            TES4FunctionArguments revisedArguments;
            function.Arguments.GetFirstAndRemoveInNew(out arg0Value, out revisedArguments);
            TES5LocalScope localScope = codeScope.LocalScope;
            ITES5Referencer newCalledOn = this.referenceFactory.CreateReadReference(arg0Value.StringValue, globalScope, multipleScriptsScope, localScope);
            const string functionName = "SetName";
            return this.objectCallFactory.CreateObjectCall(newCalledOn, functionName, this.objectCallArgumentsFactory.CreateArgumentList(revisedArguments, codeScope, globalScope, multipleScriptsScope), comment: function.Comment);
        }
    }
}