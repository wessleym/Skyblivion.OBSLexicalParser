using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
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

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return CreateLogCall(calledOn, function, globalScope);
        }

        public ITES5CodeChunk CreateLogCall(ITES5Referencer calledOn, TES4Function function, TES5GlobalScope globalScope, string? reason = null)
        {
            string message = globalScope.ScriptHeader.PrefixedScriptName + ": OBScript called " + calledOn.OutputSingle + "." + function.FunctionCall.FunctionName + "(" + string.Join(", ", function.Arguments.Select(a => a.StringValue)) + "), but script converter didn't know a conversion." + (reason != null ? "  " + reason : "");
            TES5CodeChunkCollection codeChunks = new TES5CodeChunkCollection();
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { new TES5String(message) };
            codeChunks.Add(objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Debug, "Trace", arguments, comment: function.Comment));
#if UNKNOWN_FUNCTIONS_MESSAGE_BOX
            codeChunks.Add(objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Debug, "MessageBox", arguments));//Debug.Notification might be a useful alternative.
#endif
            return codeChunks;
        }

        public ITES5Value CreateReplacement(ITES5Referencer calledOn, TES4Function function, ITES5Value replacement, TES5CodeScope codeScope, TES5GlobalScope globalScope)
        {
            codeScope.AddChunk(CreateLogCall(calledOn, function, globalScope, reason: "This call has been replaced with " + string.Join(", ", replacement.Output) + " below."));
            return replacement;
        }
    }
}