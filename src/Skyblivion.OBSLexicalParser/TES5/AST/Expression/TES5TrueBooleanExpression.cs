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
        private readonly ITES5Value value;
        public TES5TrueBooleanExpression(ITES5Value value)
        {
            this.value = value;
        }

        public ITES5Type TES5Type => TES5BasicType.T_BOOL;

        public IEnumerable<string> Output
        {
            get
            {
                TES5Bool trueBool = new TES5Bool(true);
                TES5ComparisonExpressionOperator op = TES5ComparisonExpressionOperator.OPERATOR_EQUAL;
                string outputValue = this.value.Output.Single();
                string trueOutputValue = trueBool.Output.Single();
                yield return "(" + outputValue + " " + op.Name + " " + trueOutputValue + ")";
            }
        }
    }
}
