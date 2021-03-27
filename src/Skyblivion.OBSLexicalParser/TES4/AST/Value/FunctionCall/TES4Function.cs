using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    class TES4Function : ITES4Callable, ITES4Value, ITES4CodeChunk
    {
        public TES4FunctionCall FunctionCall { get; }
        public TES4FunctionArguments Arguments { get; }
        public TES4Function(TES4FunctionCall functionCall, TES4FunctionArguments arguments)
        {
            this.FunctionCall = functionCall;
            this.Arguments = arguments;
        }

        public TES4ApiToken? CalledOn => null;

        public object Data => throw new NotImplementedException();

        public TES4Function Function => this;

        public bool HasFixedValue => false;

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.FunctionCall.Filter(predicate).Concat(this.Arguments.Filter(predicate)).ToArray();
        }
    }
}
