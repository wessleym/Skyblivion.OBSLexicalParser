using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Block
{
    class TES4BlockParameter : ITES4CodeFilterable
    {
        private string blockParameter;
        public TES4BlockParameter(string blockParameter)
        {
            this.blockParameter = blockParameter;
        }

        public string getBlockParameter()
        {
            return this.blockParameter;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeFilterable[] { this } : new ITES4CodeFilterable[] { };
        }
    }
}