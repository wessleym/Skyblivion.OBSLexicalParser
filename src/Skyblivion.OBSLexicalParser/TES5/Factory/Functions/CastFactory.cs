using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class CastFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ReferenceFactory referenceFactory;
        public CastFactory(TES5ObjectCallFactory objectCallFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.referenceFactory = referenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            string functionName = function.FunctionCall.FunctionName;
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES4StringValue value = functionArguments[0];
            ITES5Referencer toBeMethodArgument = calledOn;
            ITES5Referencer newCalledOn = this.referenceFactory.CreateReadReference(value.StringValue, globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments methodArguments = new TES5ObjectCallArguments() { toBeMethodArgument };
            ITES4StringValue? target = functionArguments.GetOrNull(1);
            if (target != null)
            {
                methodArguments.Add(this.referenceFactory.CreateReadReference(target.StringValue, globalScope, multipleScriptsScope, localScope));
            }

            return this.objectCallFactory.CreateObjectCall(newCalledOn, functionName, methodArguments);
        }
    }
}