using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5BoolCastedExpression : ITES5Expression
    {
        private readonly ITES5Value value;
        public TES5BoolCastedExpression(ITES5Value value)
        {
            this.value = value;
        }

        public ITES5Type TES5Type => this.value.TES5Type;

        public IEnumerable<string> Output => this.value.Output;
    }
}