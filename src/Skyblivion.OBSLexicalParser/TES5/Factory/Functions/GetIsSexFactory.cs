using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetIsSexFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetIsSexFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            if (function.Comment != null)
            {
                throw new ConversionException(function.FunctionCall.FunctionName + "'s comment could not be retained.");
            }
            TES4FunctionArguments functionArguments = function.Arguments;
            if (TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(calledOn.TES5Type, TES5BasicType.T_ACTOR)) { calledOn = this.objectCallFactory.CreateGetActorBase(calledOn); }
            TES5ObjectCall functionThis = this.objectCallFactory.CreateObjectCall(calledOn, "GetSex");
            int operand;
            switch (functionArguments[0].StringValue.ToLower())
            {
                case "male":
                {
                    operand = 0;
                    break;
                }

                case "female":
                {
                    operand = 1;
                    break;
                }

                default:
                {
                    throw new ConversionException("GetIsSex used with unknown gender.");
                }
            }

            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(functionThis, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Integer(operand));
            return expression;
        }
    }
}