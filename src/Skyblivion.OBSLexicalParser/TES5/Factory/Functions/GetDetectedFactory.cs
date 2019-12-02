using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetDetectedFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetDetectedFactory(TES5ReferenceFactory referenceFactory, TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.referenceFactory = referenceFactory;
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES4StringValue value = functionArguments[0];
            TES5ObjectCallArguments newArguments = new TES5ObjectCallArguments()
            {
                this.valueFactory.CreateValue(new TES4ApiToken(calledOn.Name), codeScope, globalScope, multipleScriptsScope)
            };
            ITES5Referencer newCalledOn = this.referenceFactory.CreateReadReference(value.StringValue, globalScope, multipleScriptsScope, localScope);
            const string functionName = "IsDetectedBy";
            return this.objectCallFactory.CreateObjectCall(newCalledOn, functionName, newArguments);
        }
    }
}