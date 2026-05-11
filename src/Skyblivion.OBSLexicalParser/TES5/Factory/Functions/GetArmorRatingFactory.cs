using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetArmorRatingFactory : IFunctionFactory//WTM:  Change:  Added (SKSE)
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        private const string functionName = "GetArmorRatingOfWornForm";
        public GetArmorRatingFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            globalScope.AddFunction(GetGetArmorRatingOfWornFormFunctionCodeBlock(calledOn, function.Comment));
            ITES5Value? accumulatedStatement = null;
            for (int slotMask = 0x00000004; slotMask <= 0x00000200; slotMask *= 2)//From body to shield
            {
                TES5ObjectCallCustom objectCall = GetGetArmorRatingOfWornFormObjectCall(globalScope, slotMask);
                if (accumulatedStatement == null)
                {
                    accumulatedStatement = objectCall;
                }
                else
                {
                    accumulatedStatement = TES5ExpressionFactory.CreateArithmeticExpression(accumulatedStatement, TES5ArithmeticExpressionOperator.OPERATOR_ADD, objectCall);
                }
            }
            if (accumulatedStatement == null) { throw new NullableException(nameof(accumulatedStatement)); }
            return accumulatedStatement;
        }

        private TES5FunctionCodeBlock GetGetArmorRatingOfWornFormFunctionCodeBlock(ITES5Referencer calledOn, TES4Comment? comment)
        {
            TES5FunctionScope functionScope = new TES5FunctionScope(functionName);
            TES5FunctionCodeBlock functionCodeBlock = new TES5FunctionCodeBlock(functionScope, TES5CodeScopeFactory.CreateCodeScopeRoot(functionScope), TES5BasicType.T_INT, false, false);
            TES5SignatureParameter slotMaskParameter = new TES5SignatureParameter("slotMask", TES5BasicType.T_INT, true);
            functionCodeBlock.FunctionScope.AddParameter(slotMaskParameter);
            TES5ObjectCall getWornFormObjectCall = GetGetWornFormObjectCall(calledOn, TES5ReferenceFactory.CreateReferenceToVariableOrProperty(slotMaskParameter));
            TES5Reference wornFormVariableReference = TES5LocalVariableFactory.CreateWithInitialization(TES5BasicType.T_ARMOR, "wornForm", getWornFormObjectCall, functionCodeBlock.CodeScope);
            functionCodeBlock.AddChunk(new TES5Return(objectCallFactory.CreateObjectCall(wornFormVariableReference, "GetArmorRating", comment: comment)));
            return functionCodeBlock;
        }

        private TES5ObjectCall GetGetWornFormObjectCall(ITES5Referencer calledOn, ITES5Value slotMask)
        {
            return objectCallFactory.CreateObjectCall(calledOn, "GetWornForm", new TES5ObjectCallArguments() { slotMask });
        }

        private static TES5ObjectCallCustom GetGetArmorRatingOfWornFormObjectCall(TES5GlobalScope globalScope, int slotMask)
        {
            return TES5ObjectCallFactory.CreateObjectCallCustom(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), functionName, TES5BasicType.T_INT, new TES5ObjectCallArguments() { new TES5Integer(slotMask) });
        }
    }
}
