using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ModCrimeGoldFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public ModCrimeGoldFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            ITES5Referencer newCalledOn = this.referenceFactory.CreateCyrodiilCrimeFactionReadReference(globalScope, multipleScriptsScope, localScope);
            return this.objectCallFactory.CreateObjectCall(newCalledOn, function, objectCallArgumentsFactory, codeScope, globalScope, multipleScriptsScope);
        }
    }
}