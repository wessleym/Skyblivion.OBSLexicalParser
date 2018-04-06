using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    class TES4Function : ITES4Callable, ITES4Value, ITES4CodeChunk
    {
        private TES4FunctionCall functionCall;
        private TES4FunctionArguments arguments;
        public TES4Function(TES4FunctionCall functionCall, TES4FunctionArguments arguments)
        {
            this.functionCall = functionCall;
            this.arguments = arguments;
        }

        public TES4ApiToken getCalledOn()
        {
            return null;
        }

        public TES4FunctionArguments getArguments()
        {
            return this.arguments;
        }

        public TES4FunctionCall getFunctionCall()
        {
            return this.functionCall;
        }

        public object getData()
        {
            throw new ConversionException("TES4Function.getData() - not supported");
        }

        public TES4Function getFunction()
        {
            return this;
        }

        public bool hasFixedValue()
        {
            return false;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return this.functionCall.Filter(predicate).Concat(this.arguments.filter(predicate)).ToArray();
        }
    }
}
