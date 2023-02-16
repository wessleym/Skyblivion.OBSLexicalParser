using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetActorAlphaFactory : IFunctionFactory
    {
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SetActorAlphaFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.valueFactory = valueFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES4ValueString argument0 = functionArguments[0];
            TES5ObjectCallArguments functionArgs = new TES5ObjectCallArguments()
            {
                this.valueFactory.CreateValue(argument0, codeScope, globalScope, multipleScriptsScope),
                new TES5Bool(true)
            };
            return this.objectCallFactory.CreateObjectCall(calledOn, "SetAlpha", functionArgs, comment: function.Comment);
        }
    }
}