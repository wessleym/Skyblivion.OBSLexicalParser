using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class PlaySound3DFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public PlaySound3DFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES5Referencer newCalledOn = this.referenceFactory.CreateReadReference(functionArguments[0].StringValue, globalScope, multipleScriptsScope, localScope);
            const string functionName = "play";
            TES5ObjectCallArguments args = new TES5ObjectCallArguments
            {
                calledOn//this.referenceFactory.createReferenceToSelf(globalScope)//WTM:  Change
            };
            return this.objectCallFactory.CreateObjectCall(newCalledOn, functionName, args, comment: function.Comment);
        }
    }
}