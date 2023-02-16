using Skyblivion.OBSLexicalParser.TES4.AST.Value;
using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4VariableAssignation : ITES4CodeChunk
    {
        public ITES4Reference Reference { get; }
        public ITES4Value Value { get; }
        public TES4Comment? Comment { get; private set; }
        public TES4VariableAssignation(ITES4Reference reference, ITES4Value value)
        {
            this.Reference = reference;
            this.Value = value;
            Comment = null;
        }

        public void SetComment(TES4Comment comment)
        {
            if (Comment != null) { throw new InvalidOperationException(nameof(Comment) + " was already set."); }
            Comment = comment;
        }
    }
}