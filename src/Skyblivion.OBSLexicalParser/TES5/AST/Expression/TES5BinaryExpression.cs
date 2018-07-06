using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    abstract class TES5BinaryExpression<T> : ITES5Expression where T : TES5ExpressionOperator
    {
        protected ITES5Value LeftValue { get; private set; }
        private readonly T op;
        protected ITES5Value RightValue { get; private set; }
        public TES5BinaryExpression(ITES5Value leftValue, T op, ITES5Value rightValue)
        {
            this.LeftValue = leftValue;
            this.op = op;
            this.RightValue = rightValue;
        }

        public IEnumerable<string> Output
        {
            get
            {
                string leftOutput = this.LeftValue.Output.Single();
                string rightOutput = this.RightValue.Output.Single();
                yield return "(" + leftOutput + " " + this.op.Name + " " + rightOutput + ")";
            }
        }

        public abstract ITES5Type TES5Type { get; }
    }
}
