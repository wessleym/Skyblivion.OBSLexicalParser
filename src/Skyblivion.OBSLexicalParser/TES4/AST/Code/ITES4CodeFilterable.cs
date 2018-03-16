using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    interface ITES4CodeFilterable
    {
        ITES4CodeFilterable[] filter(Func<ITES4CodeFilterable, bool> predicate);
    }
}
