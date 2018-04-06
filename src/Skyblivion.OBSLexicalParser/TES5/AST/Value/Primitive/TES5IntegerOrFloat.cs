using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    abstract class TES5IntegerOrFloat : ITES5Primitive
    {
        public abstract ITES5Type TES5Type { get; }
        public abstract IEnumerable<string> Output { get; }
        public abstract int ConvertedIntValue { get; }
    }
}
