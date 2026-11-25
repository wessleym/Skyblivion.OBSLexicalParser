using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    internal static class SpeedMultFactory
    {
        private const int walkSpeedAtZeroSpeedAttribute = 90;
        private const float walkSpeedPerSpeedAttributePoint = .4f;
        private const int normalSpeedAttribute = 40;
        private const float normalWalkSpeed = walkSpeedAtZeroSpeedAttribute + walkSpeedPerSpeedAttributePoint * normalSpeedAttribute;
        private const int speedMultAtNormal = 100;
        private const float carryWeightNudge = .1f;

        private static TES5ArithmeticExpression CreateArithmeticExpression(ITES5Value walkSpeed)
        {
            return TES5ExpressionFactory.CreateArithmeticExpression(
                new TES5Integer(speedMultAtNormal),
                TES5ArithmeticExpressionOperator.OPERATOR_MULTIPLY,
                    TES5ExpressionFactory.CreateArithmeticExpression(
                        walkSpeed,
                        TES5ArithmeticExpressionOperator.OPERATOR_DIVIDE,
                        new TES5Float(normalWalkSpeed)
                        )
                );
        }

        private static TES5CodeChunkCollection CreateCodeChunks(TES5ObjectCallFactory objectCallFactory, ITES5Referencer calledOn, string newFunctionName, TES4Function function, ITES5Value speedMult)
        {
            return new TES5CodeChunkCollection()
            {
                objectCallFactory.CreateObjectCall(calledOn, newFunctionName, new TES5ObjectCallArguments() { new TES5String("SpeedMult"), speedMult }, function.Comment),
                // A SpeedMult change is applied only when the actor's carry weight changes (or it next starts or stops sprinting or walking).
                // The carry weight is modified to apply the SpeedMult change immediately.
                objectCallFactory.CreateObjectCall(calledOn, "ModActorValue", new TES5ObjectCallArguments() { new TES5String("CarryWeight"), new TES5Float(carryWeightNudge) }, null),
                objectCallFactory.CreateObjectCall(calledOn, "ModActorValue", new TES5ObjectCallArguments() { new TES5String("CarryWeight"), new TES5Float(-carryWeightNudge) }, null)
            };
        }

        private static TES5CodeChunkCollection CreateCodeChunks(ITES5Value walkSpeed, TES5ObjectCallFactory objectCallFactory, ITES5Referencer calledOn, string newFunctionName, TES4Function function)
        {
            TES5ArithmeticExpression newArgument = CreateArithmeticExpression(walkSpeed);
            return CreateCodeChunks(objectCallFactory, calledOn, newFunctionName, function, newArgument);
        }

        public static TES5CodeChunkCollection CreateSetActorValueCodeChunks(ITES5Referencer calledOn, TES4Function function, ITES5Value newSpeedAttribute, TES5ObjectCallFactory objectCallFactory)
        {
            TES5ArithmeticExpression walkSpeed = TES5ExpressionFactory.CreateArithmeticExpression(
                new TES5Integer(walkSpeedAtZeroSpeedAttribute),
                TES5ArithmeticExpressionOperator.OPERATOR_ADD,
                TES5ExpressionFactory.CreateArithmeticExpression(
                    new TES5Float(walkSpeedPerSpeedAttributePoint),
                    TES5ArithmeticExpressionOperator.OPERATOR_MULTIPLY,
                    newSpeedAttribute),
                outputParentheses: true
            );
            return CreateCodeChunks(walkSpeed, objectCallFactory, calledOn, "SetActorValue", function);
        }

        public static TES5CodeChunkCollection CreateModActorValueCodeChunks(ITES5Referencer calledOn, TES4Function function, ITES5Value newSpeedAttribute, TES5ObjectCallFactory objectCallFactory)
        {
            TES5ArithmeticExpression walkSpeed = TES5ExpressionFactory.CreateArithmeticExpression(
                new TES5Float(walkSpeedPerSpeedAttributePoint),
                TES5ArithmeticExpressionOperator.OPERATOR_MULTIPLY,
                newSpeedAttribute
            );
            return CreateCodeChunks(walkSpeed, objectCallFactory, calledOn, "ModActorValue", function);
        }

        public static TES5ArithmeticExpression CreateGetActorValue(ITES5Referencer calledOn, TES5ObjectCallFactory objectCallFactory, bool useBase)
        {
            TES5ObjectCall getActorValue = objectCallFactory.CreateObjectCall(calledOn, useBase ? "GetBaseActorValue" : "GetActorValue", new TES5ObjectCallArguments() { new TES5String("SpeedMult") });
            return TES5ExpressionFactory.CreateArithmeticExpression(
                TES5ExpressionFactory.CreateArithmeticExpression(
                    TES5ExpressionFactory.CreateArithmeticExpression(
                        TES5ExpressionFactory.CreateArithmeticExpression(
                            getActorValue,
                            TES5ArithmeticExpressionOperator.OPERATOR_DIVIDE,
                            new TES5Integer(speedMultAtNormal)
                        ),
                        TES5ArithmeticExpressionOperator.OPERATOR_MULTIPLY,
                        new TES5Float(normalWalkSpeed)
                    ),
                    TES5ArithmeticExpressionOperator.OPERATOR_SUBSTRACT,
                    new TES5Integer(walkSpeedAtZeroSpeedAttribute),
                    outputParentheses: true
                ),
                TES5ArithmeticExpressionOperator.OPERATOR_DIVIDE,
                new TES5Float(walkSpeedPerSpeedAttributePoint)
            );
        }
    }
}
