using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5Filler : ITES5ValueCodeChunk
    {
        public IEnumerable<string> output()
        {
            return new string[] { "" };
        }

        public ITES5Type getType()//WTM:  Change:  Added until a new proper interface is made.
        {
            throw new NotImplementedException();
        }
    }
}