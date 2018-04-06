using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Code
{
    class TES5Filler : ITES5ValueCodeChunk
    {
        public IEnumerable<string> Output => new string[] { "" };

        public ITES5Type TES5Type => throw new NotImplementedException();//WTM:  Change:  Added until a new proper interface is made.
    }
}