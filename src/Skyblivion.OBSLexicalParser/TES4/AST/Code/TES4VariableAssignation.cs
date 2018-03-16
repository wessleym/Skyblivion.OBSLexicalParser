using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4VariableAssignation : ITES4CodeChunk
    {
        private ITES4Reference reference;
        private ITES4Value value;
        public TES4VariableAssignation(ITES4Reference reference, ITES4Value value)
        {
            this.reference = reference;
            this.value = value;
        }

        public ITES4Reference getReference()
        {
            return this.reference;
        }

        public ITES4Value getValue()
        {
            return this.value;
        }

        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            IEnumerable<ITES4CodeFilterable> filtered = new List<ITES4CodeFilterable>();
            if (predicate(this.reference))
            {
                filtered = filtered.Concat(new ITES4CodeFilterable[] { this.reference });
            }
            filtered = filtered.Concat(this.value.filter(predicate));
            return filtered.ToArray();
        }
    }
}