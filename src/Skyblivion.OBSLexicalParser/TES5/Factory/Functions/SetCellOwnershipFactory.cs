using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetCellOwnershipFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public SetCellOwnershipFactory(TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            const string functionName = "SetActorOwner";
            TES5LocalScope localScope = codeScope.LocalScope;
            ITES4StringValue tes4Cell = function.Arguments[0];
            ITES5Referencer tes5Cell = this.referenceFactory.CreateReference(tes4Cell.StringValue, globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments()
            {
                objectCallFactory.CreateGetActorBaseOfPlayer(globalScope, multipleScriptsScope)
            };
            return this.objectCallFactory.CreateObjectCall(tes5Cell, functionName, multipleScriptsScope, arguments);
        }
    }
}