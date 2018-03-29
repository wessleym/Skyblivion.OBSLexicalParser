using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5ArithmeticExpression : ITES5Expression
    {
        private ITES5Value leftValue;
        private TES5ArithmeticExpressionOperator op;
        private ITES5Value rightValue;
        public TES5ArithmeticExpression(ITES5Value leftValue, TES5ArithmeticExpressionOperator op, ITES5Value rightValue)
        {
            this.leftValue = leftValue;
            this.op=op;
            this.rightValue = rightValue;
        }

        public List<string> output()
        {
            string leftOutput = this.leftValue.output().Single();
            string rightOutput = this.rightValue.output().Single();
            return new List<string>() { "(" + leftOutput + " " + this.op.Name + " " + rightOutput + ")" };
        }

        public ITES5Type getType()
        {
            return TES5BasicType.T_BOOL;
        }
    }
}