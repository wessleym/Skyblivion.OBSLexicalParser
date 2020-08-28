using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class CreateFullActorCopyFactory : IFunctionFactory
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public CreateFullActorCopyFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //We move the called upon to function arg ( cloned object ) and we replace placed upon to player
            ITES5Referencer newCalledOn = TES5ReferenceFactory.CreateReferenceToPlayer(globalScope);
            const string functionName = "PlaceAtMe";
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments()
            {
                calledOn,
                new TES5Integer(1),//WTM:  Change:  I added this argument.
                new TES5Bool(true)
            };
            return this.objectCallFactory.CreateObjectCall(newCalledOn, functionName,                 //this.objectCallArgumentsFactory.createArgumentList(functionArguments, codeScope, globalScope, multipleScriptsScope)//WTM:  Change:  Previously, the above arguments variable was not used.  It probably should be.
                arguments);
        }
    }
}