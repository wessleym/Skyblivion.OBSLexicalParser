using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class LogUnknownFunctionFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5StaticReferenceFactory staticReferenceFactory;
        public LogUnknownFunctionFactory(TES5ObjectCallFactory objectCallFactory, TES5StaticReferenceFactory staticReferenceFactory)
        {
            this.objectCallFactory = objectCallFactory;
            this.staticReferenceFactory = staticReferenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            string message = "OBScript called " + function.FunctionCall.FunctionName + "(" + string.Join(", ", function.Arguments) + "), but script converter didn't know a conversion.";
            TES5CodeChunkCollection codeChunks = new TES5CodeChunkCollection();
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { new TES5String(message) };
            codeChunks.Add(objectCallFactory.CreateObjectCall(staticReferenceFactory.Debug, "Trace", arguments));
#if UNKNOWN_FUNCTIONS_MESSAGE_BOX
            codeChunks.Add(objectCallFactory.CreateObjectCall(staticReferenceFactory.Debug, "MessageBox", arguments));//Debug.Notification might be a useful alternative.
#endif
            return codeChunks;
        }
    }
}