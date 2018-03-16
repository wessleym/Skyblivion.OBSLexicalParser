using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    class TES4Return : ITES4CodeChunk
    {
        public ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate)
        {
            return predicate(this) ? new ITES4CodeChunk[] { this } : new ITES4CodeChunk[] { };
        }
    }
}
