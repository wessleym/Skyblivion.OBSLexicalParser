using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess
{
    class TES4ObjectCall : ITES4Callable, ITES4Value, ITES4CodeChunk, ITES4CodeChunkContainer
    {
        public TES4ApiToken CalledOn { get; }
        public TES4Function Function { get; }
        public TES4ObjectCall(TES4ApiToken apiToken, TES4Function function)
        {
            this.CalledOn = apiToken;
            this.Function = function;
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            IEnumerable<ITES4CodeChunk> enumerable = this.CalledOn;
            enumerable = enumerable.Concat(this.Function);
            return enumerable.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
