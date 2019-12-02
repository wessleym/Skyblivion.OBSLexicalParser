using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetPCFameFactory : IFunctionFactory
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SetPCFameFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            //This has to be a write-action reference, not a read reference!
            ITES5Referencer fameReference = this.referenceFactory.CreateReference("Fame", globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments fameArguments = new TES5ObjectCallArguments()
            {
                this.valueFactory.CreateValue(functionArguments[0], codeScope, globalScope, multipleScriptsScope)
            };
            TES5ObjectCall newFunction = this.objectCallFactory.CreateObjectCall(fameReference, "SetValue", fameArguments);
            return newFunction;
        }
    }
}