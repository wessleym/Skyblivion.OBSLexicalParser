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
        private TES4ApiToken called;
        private TES4Function function;
        public TES4ObjectCall(TES4ApiToken apiToken, TES4Function function)
        {
            this.called = apiToken;
            this.function = function;
        }

        public TES4ApiToken CalledOn => this.called;

        public TES4Function Function => this.function;

        public object Data => StringValue;

        public string StringValue => throw new NotImplementedException(nameof(TES4ObjectCall) + "." + nameof(StringValue) + " is not supported.");

        public bool HasFixedValue => false;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.called.Filter(predicate).Concat(this.function.Filter(predicate)).ToArray();
        }
    }
}
