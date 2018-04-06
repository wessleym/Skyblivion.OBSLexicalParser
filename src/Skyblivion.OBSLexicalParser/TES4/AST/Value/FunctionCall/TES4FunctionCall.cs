using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    class TES4FunctionCall : ITES4CodeFilterable
    {
        private string functionName;
        public TES4FunctionCall(string functionName)
        {
            this.functionName = functionName;
        }

        public string getFunctionName()
        {
            return this.functionName;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}