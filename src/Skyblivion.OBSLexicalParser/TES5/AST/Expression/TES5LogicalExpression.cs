using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    class TES5LogicalExpression : ITES5Expression
    {
        private ITES5Value leftValue;
        private ITES5Value rightValue;
        private TES5LogicalExpressionOperator op;
        public TES5LogicalExpression(ITES5Value left, TES5LogicalExpressionOperator op, ITES5Value right)
        {
            this.leftValue = left;
            this.op = op;
            this.rightValue = right;
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