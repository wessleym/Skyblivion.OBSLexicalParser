using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess
{
    //WTM:  Note:  Technically, this should not implement ITES4StringValue because it doesn't implement StringValue.
    //But if this class does not implement ITES4StringValue, exceptions are thrown in TES4ObscriptCodeGrammar.
    class TES4ObjectCall : ITES4Callable, ITES4CodeChunk, ITES4StringValue
    {
        public TES4ApiToken CalledOn { get; private set; }
        public TES4Function Function { get; private set; }
        public TES4ObjectCall(TES4ApiToken apiToken, TES4Function function)
        {
            this.CalledOn = apiToken;
            this.Function = function;
        }

        public object Data => StringValue;

        public string StringValue => throw new NotImplementedException(nameof(TES4ObjectCall) + "." + nameof(StringValue) + " is not supported.");

        public bool HasFixedValue => false;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.CalledOn.Filter(predicate).Concat(this.Function.Filter(predicate)).ToArray();
        }
    }
}
