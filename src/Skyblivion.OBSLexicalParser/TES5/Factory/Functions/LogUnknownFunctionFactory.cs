using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class LogUnknownFunctionFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public LogUnknownFunctionFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return CreateLogCall(function);
        }

        public ITES5ValueCodeChunk CreateLogCall(TES4Function function, string? reason = null)
        {
            string message = "OBScript called " + function.FunctionCall.FunctionName + "(" + string.Join(", ", function.Arguments.Select(a => a.StringValue)) + "), but script converter didn't know a conversion." + (reason != null ? "  " + reason : "");
            TES5CodeChunkCollection codeChunks = new TES5CodeChunkCollection();
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { new TES5String(message) };
            codeChunks.Add(objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Debug, "Trace", arguments, comment: function.Comment));
#if UNKNOWN_FUNCTIONS_MESSAGE_BOX
            codeChunks.Add(objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Debug, "MessageBox", arguments));//Debug.Notification might be a useful alternative.
#endif
            return codeChunks;
        }

        public ITES5ValueCodeChunk CreateReplacement(TES4Function function, ITES5ValueCodeChunk replacement, TES5CodeScope codeScope)
        {
            codeScope.AddChunk(CreateLogCall(function, reason: "This call has been replaced with " + string.Join(", ", replacement.Output) + " below."));
            return replacement;
        }
    }
}