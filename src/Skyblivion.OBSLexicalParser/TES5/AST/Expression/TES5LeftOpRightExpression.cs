using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    abstract class TES5LeftOpRightExpression<T> : ITES5Expression where T : TES5ExpressionOperator
    {
        private ITES5Value leftValue;
        private T op;
        private ITES5Value rightValue;
        public TES5LeftOpRightExpression(ITES5Value leftValue, T op, ITES5Value rightValue)
        {
            this.leftValue = leftValue;
            this.op = op;
            this.rightValue = rightValue;
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

        public abstract ITES5Type TES5Type { get; }
    }
}
