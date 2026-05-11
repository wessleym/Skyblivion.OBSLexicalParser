using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    class TES4Function : ITES4Callable, ITES4CodeChunk, ITES4Value, ITES4CodeChunkContainer
    {
        public TES4FunctionCall FunctionCall { get; }
        public TES4FunctionArguments Arguments { get; }
        public TES4Comment? Comment { get; private set; }
        public TES4Function(TES4FunctionCall functionCall, TES4FunctionArguments arguments)
        {
            this.FunctionCall = functionCall;
            this.Arguments = arguments;
            this.Comment = null;
        }

        public TES4ApiToken? CalledOn => null;

        public TES4Function Function => this;

        public void SetComment(TES4Comment comment)
        {
            if (Comment != null) { throw new InvalidOperationException(nameof(Comment) + " had already been set."); }
            Comment = comment;
        }

        public IEnumerator<ITES4CodeChunk> GetEnumerator()
        {
            return new ITES4CodeChunk[] { this.FunctionCall }.Concat(this.Arguments).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
