using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class SetIgnoreFriendlyHitsFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public SetIgnoreFriendlyHitsFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            int willIgnore = ((TES4Integer)function.Arguments[0]).IntValue;
            bool ignore = willIgnore != 0;
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { new TES5Bool(ignore) };
            return objectCallFactory.CreateObjectCall(calledOn, "IgnoreFriendlyHits", arguments);
        }
    }
}
