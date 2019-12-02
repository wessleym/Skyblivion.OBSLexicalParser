using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class IsPlayerInJailFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectPropertyFactory objectPropertyFactory;
        public IsPlayerInJailFactory(TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectPropertyFactory = objectPropertyFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            //Using Legacy TES4 Connector Plugin
            return this.objectPropertyFactory.CreateObjectProperty(multipleScriptsScope, this.referenceFactory.CreateContainerReadReference(globalScope, multipleScriptsScope, localScope), "isInJail");
        }
    }
}