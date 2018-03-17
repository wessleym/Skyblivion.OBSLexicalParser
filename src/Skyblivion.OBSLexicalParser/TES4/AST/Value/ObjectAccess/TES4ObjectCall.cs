using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.ObjectAccess
{
    class TES4ObjectCall : ITES4Callable, ITES4ObjectAccess, ITES4CodeChunk
    {
        private TES4ApiToken called;
        private TES4Function function;
        public TES4ObjectCall(TES4ApiToken apiToken, TES4Function function)
        {
            this.called = apiToken;
            this.function = function;
        }

        public TES4ApiToken getCalledOn()
        {
            return this.called;
        }

        public TES4Function getFunction()
        {
            return this.function;
        }

        public object getData()
        {
            return StringValue;
        }

        public string StringValue => throw new InvalidOperationException(nameof(TES4ObjectCall) + "." + nameof(StringValue) + " is not supported.");

        public bool hasFixedValue()
        {
            return false;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.called.filter(predicate).Concat(this.function.filter(predicate)).ToArray();
        }
    }
}
