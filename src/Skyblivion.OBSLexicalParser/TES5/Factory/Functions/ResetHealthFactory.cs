using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ResetHealthFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public ResetHealthFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //Healing to full hp?
            const string functionName = "RestoreActorValue";
            TES5ObjectCallArguments convertedArguments = new TES5ObjectCallArguments() { new TES5String("Health"), new TES5Integer(9999) };
            return this.objectCallFactory.CreateObjectCall(calledOn, functionName, convertedArguments, comment: function.Comment);
        }
    }
}