using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4SubBranch : ITES4CodeFilterable
    {
        private ITES4Value expression;
        private TES4CodeChunks codeChunks;
        public TES4SubBranch(ITES4Value expression, TES4CodeChunks codeChunks = null)
        {
            this.expression = expression;
            this.codeChunks = codeChunks;
        }

        public TES4CodeChunks getCodeChunks()
        {
            return this.codeChunks;
        }

        public ITES4Value getExpression()
        {
            return this.expression;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = this.expression.filter(predicate);
            if (this.codeChunks != null)
            {
                filtered = filtered.Concat(this.codeChunks.filter(predicate));
            }
            return filtered.ToArray();
        }
    }
}