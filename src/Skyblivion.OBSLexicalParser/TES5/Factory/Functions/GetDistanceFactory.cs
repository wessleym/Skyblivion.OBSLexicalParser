using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetDistanceFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetDistanceFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            string functionName = function.FunctionCall.FunctionName;
            TES4FunctionArguments functionArguments = function.Arguments;
            //Sometimes, its referenced as a string in code, so we force cast it to a reference.
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments()
            {
                this.referenceFactory.CreateReadReference(functionArguments[0].StringValue, globalScope, multipleScriptsScope, codeScope.LocalScope)
            };
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, arguments, comment: function.Comment);
        }
    }
}