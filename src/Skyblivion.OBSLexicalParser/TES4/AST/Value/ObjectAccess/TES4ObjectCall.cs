using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess
{
    class TES4ObjectCall : ITES4Callable, ITES4Value, ITES4CodeChunk
    {
        public TES4ApiToken CalledOn { get; }
        public TES4Function Function { get; }
        public TES4ObjectCall(TES4ApiToken apiToken, TES4Function function)
        {
            this.CalledOn = apiToken;
            this.Function = function;
        }
    }
}
