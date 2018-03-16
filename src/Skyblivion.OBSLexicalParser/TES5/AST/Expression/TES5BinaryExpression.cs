using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;

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

        public List<string> output()
        {
            List<string> leftOutput = this.leftValue.output();
            string leftOutputFirst = leftOutput[0];
            List<string> rightOutput = this.rightValue.output();
            string rightOutputFirst = rightOutput[0];
            return new List<string>() { "(" + leftOutputFirst + " " + this.op.Name + " " + rightOutputFirst + ")" };
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