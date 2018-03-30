using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5Return : ITES5CodeChunk
    {
        public IEnumerable<string> output()
        {
            return new string[] { "Return" };
        }
    }
}