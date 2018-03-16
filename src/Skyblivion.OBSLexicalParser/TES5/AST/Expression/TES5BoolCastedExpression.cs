using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5BoolCastedExpression : ITES5Expression
    {
        private ITES5Value value;
        public TES5BoolCastedExpression(ITES5Value value)
        {
            this.value = value;
        }

        public ITES5Type getType()
        {
            return this.value.getType();
        }

        public List<string> output()
        {
            return this.value.output();
        }
    }
}