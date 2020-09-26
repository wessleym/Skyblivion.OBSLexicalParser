using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
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

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            globalScope.AddFunction(GetGetArmorRatingOfWornFormFunctionCodeBlock(calledOn, codeScope));
            ITES5ValueCodeChunk? accumulatedStatement = null;
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

        private TES5FunctionCodeBlock GetGetArmorRatingOfWornFormFunctionCodeBlock(ITES5Referencer calledOn, TES5CodeScope codeScope)
        {
            TES5FunctionCodeBlock functionCodeBlock = new TES5FunctionCodeBlock(new TES5FunctionScope(functionName), TES5CodeScopeFactory.CreateCodeScope(codeScope.LocalScope), TES5BasicType.T_INT, false, false);
            TES5SignatureParameter slotMaskParameter = new TES5SignatureParameter("slotMask", TES5BasicType.T_INT, true);
            functionCodeBlock.FunctionScope.AddVariable(slotMaskParameter);
            TES5ObjectCall getWornForm = GetGetWornFormObjectCall(calledOn, TES5ReferenceFactory.CreateReferenceToVariableOrProperty(slotMaskParameter));
            TES5LocalVariable wornFormVariable = new TES5LocalVariable("wornForm", TES5BasicType.T_ARMOR);
            functionCodeBlock.CodeScope.AddVariable(wornFormVariable);
            TES5Reference wornFormVariableReference = TES5ReferenceFactory.CreateReferenceToVariableOrProperty(wornFormVariable);
            functionCodeBlock.AddChunk(TES5VariableAssignationFactory.CreateAssignation(wornFormVariableReference, getWornForm));
            functionCodeBlock.AddChunk(new TES5Return(objectCallFactory.CreateObjectCall(wornFormVariableReference, "GetArmorRating")));
            return functionCodeBlock;
        }

        private TES5ObjectCall GetGetWornFormObjectCall(ITES5Referencer calledOn, ITES5Value slotMask)
        {
            return objectCallFactory.CreateObjectCall(calledOn, "GetWornForm", new TES5ObjectCallArguments() { slotMask });
        }

        private TES5ObjectCallCustom GetGetArmorRatingOfWornFormObjectCall(TES5GlobalScope globalScope, int slotMask)
        {
            return objectCallFactory.CreateObjectCallCustom(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), functionName, TES5BasicType.T_INT, new TES5ObjectCallArguments() { new TES5Integer(slotMask) });
        }
    }
}
