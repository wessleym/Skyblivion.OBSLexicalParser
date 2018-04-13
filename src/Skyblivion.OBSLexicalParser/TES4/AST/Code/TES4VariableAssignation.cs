using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4VariableAssignation : ITES4CodeChunk
    {
        public ITES4Reference Reference { get; private set; }
        public ITES4Value Value { get; private set; }
        public TES4VariableAssignation(ITES4Reference reference, ITES4Value value)
        {
            this.Reference = reference;
            this.Value = value;
        }

        public ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = new ITES4CodeFilterable[] { };
            if (predicate(this.Reference))
            {
                filtered = filtered.Concat(new ITES4CodeFilterable[] { this.Reference });
            }
            filtered = filtered.Concat(this.Value.Filter(predicate));
            return filtered.ToArray();
        }
    }
}