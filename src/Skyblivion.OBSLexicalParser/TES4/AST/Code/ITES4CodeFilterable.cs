using System;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Code
{
    interface ITES4CodeFilterable
    {
        ITES4CodeFilterable[] Filter(Func<ITES4CodeFilterable, bool> predicate);
    }
}
