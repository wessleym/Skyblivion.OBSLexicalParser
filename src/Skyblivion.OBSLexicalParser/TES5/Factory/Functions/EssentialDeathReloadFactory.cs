using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    internal class EssentialDeathReloadFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public EssentialDeathReloadFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            string text = function.Arguments[0].StringValue;
            return new TES5CodeChunkCollection()
            {
                objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Debug, "MessageBox", arguments: [new TES5String(text)], function.Comment),
                objectCallFactory.CreateObjectCall(new TES5PlayerReference(), "Kill")
            };
        }
    }
}
