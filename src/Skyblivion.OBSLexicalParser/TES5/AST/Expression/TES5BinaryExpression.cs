using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5BinaryExpression : ITES5Expression
    {
        private ITES5Value leftValue;
        private TES5BinaryExpressionOperator op;
        private ITES5Value rightValue;
        public TES5BinaryExpression(ITES5Value left, TES5BinaryExpressionOperator  op, ITES5Value right)
        {
            this.leftValue = left;
            this.op = op;
            this.rightValue = right;
        }

        public IEnumerable<string> output()
        {
            string leftOutput = this.leftValue.output().Single();
            string rightOutput = this.rightValue.output().Single();
            return new string[] { "(" + leftOutput + " " + this.op.Name + " " + rightOutput + ")" };
        }

        public ITES5Type getType()
        {
            if (this.leftValue.getType() == TES5BasicType.T_FLOAT || this.rightValue.getType() == TES5BasicType.T_FLOAT)
            {
                return TES5BasicType.T_FLOAT;
            }
            return TES5BasicType.T_INT;
        }
    }
}