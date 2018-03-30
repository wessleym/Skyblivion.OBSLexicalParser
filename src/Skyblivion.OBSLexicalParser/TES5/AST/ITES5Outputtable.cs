using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST
{
    interface ITES5Outputtable
    {
        IEnumerable<string> output();
    }
}