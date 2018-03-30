using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5TrueBooleanExpression : ITES5Expression
    {
        private ITES5Value value;
        public TES5TrueBooleanExpression(ITES5Value value)
        {
            this.value = value;
        }

        public ITES5Type getType()
        {
            return TES5BasicType.T_BOOL;
        }

        public IEnumerable<string> output()
        {
            TES5Bool trueBool = new TES5Bool(true);
            TES5ArithmeticExpressionOperator op = TES5ArithmeticExpressionOperator.OPERATOR_EQUAL;
            string outputValue = this.value.output().Single();
            string trueOutputValue = trueBool.output().Single();
            return new string[] { "(" + outputValue + " " + op.Name + " " + trueOutputValue + ")" };
        }
    }
}
