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
        public TES5ArithmeticExpression(ITES5Value left, TES5ArithmeticExpressionOperator  op, ITES5Value right)
        {
            this.leftValue = left;
            this.op = op;
            this.rightValue = right;
        }

        public IEnumerable<string> Output
        {
            get
            {
                string leftOutput = this.leftValue.Output.Single();
                string rightOutput = this.rightValue.Output.Single();
                return new string[] { "(" + leftOutput + " " + this.op.Name + " " + rightOutput + ")" };
            }
        }

        public ITES5Type TES5Type
        {
            get
            {
                if (this.leftValue.TES5Type == TES5BasicType.T_FLOAT || this.rightValue.TES5Type == TES5BasicType.T_FLOAT)
                {
                    return TES5BasicType.T_FLOAT;
                }
                return TES5BasicType.T_INT;
            }
        }
    }
}