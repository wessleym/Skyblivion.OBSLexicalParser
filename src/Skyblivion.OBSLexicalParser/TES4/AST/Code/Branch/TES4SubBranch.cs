using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code.Branch
{
    class TES4SubBranch : ITES4CodeFilterable
    {
        public ITES4Value Expression { get; private set; }
        public TES4CodeChunks? CodeChunks { get; private set; }
        public TES4SubBranch(ITES4Value expression, TES4CodeChunks? codeChunks = null)
        {
            this.Expression = expression;
            this.CodeChunks = codeChunks;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = this.Expression.Filter(predicate);
            if (this.CodeChunks != null)
            {
                filtered = filtered.Concat(this.CodeChunks.Filter(predicate));
            }
            return filtered.ToArray();
        }
    }
}