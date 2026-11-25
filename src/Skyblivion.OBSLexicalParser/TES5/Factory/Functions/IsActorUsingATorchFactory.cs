using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    internal class IsActorUsingATorchFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public IsActorUsingATorchFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5ObjectCall getEquippedItemType = objectCallFactory.CreateObjectCall(calledOn, "GetEquippedItemType", new TES5ObjectCallArguments() { new TES5Integer(0) });
            return TES5ExpressionFactory.CreateComparisonExpression(getEquippedItemType, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Integer(11));
        }
    }
}
