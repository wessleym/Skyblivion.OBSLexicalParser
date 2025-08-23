using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Expression
{
    abstract class TES5BinaryExpression : ITES5Expression
    {
        public ITES5Value LeftValue { get; }
        public TES5ExpressionOperator Operator { get; }
        public ITES5Value RightValue { get; }
        public TES5BinaryExpression(ITES5Value leftValue, TES5ExpressionOperator op, ITES5Value rightValue)
        {
            this.LeftValue = leftValue;
            this.Operator = op;
            this.RightValue = rightValue;
        }

        private bool IsAffirmative()
        {
            // == false -> true false  -> false
            // == true  -> true true   -> true
            // != false -> false false -> true
            // != true  -> false true  -> false
            // ==  true  -> true  true true   -> true
            // == !true  -> true  false true  -> false
            // !=  true  -> false true true   -> false
            // != !false -> false false false -> false
            var leftBoolean = LeftValue as TES5Bool;
            var rightBoolean = RightValue as TES5Bool;
            if (leftBoolean != null || rightBoolean != null)
            {
                bool opEqual =
                    this.Operator == TES5ComparisonExpressionOperator.OPERATOR_EQUAL ? true :
                    this.Operator == TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL ? false :
                    throw new InvalidOperationException("Invalid operator: " + this.Operator.Name);
                bool boolean = leftBoolean != null ? leftBoolean.Value : rightBoolean!.Value;
                TES5BinaryExpression? otherBinaryExpression = LeftValue as TES5BinaryExpression ?? RightValue as TES5BinaryExpression;
                bool otherBoolean = otherBinaryExpression == null || otherBinaryExpression.IsAffirmative();
                return opEqual == boolean == otherBoolean;
            }
            return true;
        }

        public string GetOutputSingle(bool allowNegation)
        {//This is an effort to reduce needless output (e.g., "False == True" -> "False").
            var leftBoolean = LeftValue as TES5Bool;
            var rightBoolean = RightValue as TES5Bool;
            if (leftBoolean != null || rightBoolean != null)
            {
                if (this.Operator == TES5LogicalExpressionOperator.OPERATOR_AND)
                {
                    if (leftBoolean != null && leftBoolean.Value)
                    {
                        return RightValue.OutputSingle;
                    }
                    if (rightBoolean != null && rightBoolean.Value)
                    {
                        return LeftValue.OutputSingle;
                    }
                    throw new ConversionException(typeof(TES5LogicalExpressionOperator).FullName + " unhandled");
                }
                if (this.Operator == TES5LogicalExpressionOperator.OPERATOR_OR)
                {
                    if (leftBoolean != null && !leftBoolean.Value)
                    {
                        return RightValue.OutputSingle;
                    }
                    if (rightBoolean != null && !rightBoolean.Value)
                    {
                        return LeftValue.OutputSingle;
                    }
                    throw new ConversionException(typeof(TES5LogicalExpressionOperator).FullName + " unhandled");
                }
                bool shouldBeTrue = this.IsAffirmative();
                var potentiallyNonBooleanValue = leftBoolean != null ? RightValue : LeftValue;
                var comparisonExpression = potentiallyNonBooleanValue as TES5ComparisonExpression;
                bool affirm = !allowNegation || shouldBeTrue;
                if (!affirm && comparisonExpression != null && (comparisonExpression.Operator == TES5ComparisonExpressionOperator.OPERATOR_EQUAL || comparisonExpression.Operator == TES5ComparisonExpressionOperator.OPERATOR_NOT_EQUAL))
                {
                    return TES5ExpressionFactory.CreateComparisonExpression(comparisonExpression.LeftValue, ((TES5ComparisonExpressionOperator)comparisonExpression.Operator).Flip(), comparisonExpression.RightValue).OutputSingle;
                }
                if (leftBoolean != null && rightBoolean != null)
                {
                    if (leftBoolean.AllowSimplification && rightBoolean.AllowSimplification)
                    {
                        return new TES5Bool(this.Operator == TES5ComparisonExpressionOperator.OPERATOR_EQUAL == leftBoolean.Value == rightBoolean.Value).OutputSingle;
                    }
                    if (leftBoolean.AllowSimplification)
                    {
                        affirm = !allowNegation || (this.Operator == TES5ComparisonExpressionOperator.OPERATOR_EQUAL == leftBoolean.Value);
                        return (affirm ? "" : "!") + rightBoolean.OutputSingle;
                    }
                    if (rightBoolean.AllowSimplification)
                    {
                        affirm = !allowNegation || (this.Operator == TES5ComparisonExpressionOperator.OPERATOR_EQUAL == rightBoolean.Value);
                        return (affirm ? "" : "!") + leftBoolean.OutputSingle;
                    }
                }
                else
                {
                    var binaryExpression = potentiallyNonBooleanValue as TES5BinaryExpression;
                    return (affirm ? "" : "!") + (binaryExpression != null ? binaryExpression.GetOutputSingle(false) : potentiallyNonBooleanValue.OutputSingle);
                }
            }
            string leftOutput = this.LeftValue.OutputSingle;
            string rightOutput = this.RightValue.OutputSingle;
            if (this is TES5ArithmeticExpression)
            {
                if (this.LeftValue is TES5ArithmeticExpression)
                {
                    leftOutput = "(" + leftOutput + ")";
                }
                if (this.RightValue is TES5ArithmeticExpression)
                {
                    rightOutput = "(" + rightOutput + ")";
                }
            }
            return leftOutput + " " + this.Operator.Name + " " + rightOutput;
        }

        public string OutputSingle => GetOutputSingle(true);

        public IEnumerable<string> Output => [OutputSingle];

        public abstract ITES5Type TES5Type { get; }
    }
}
