using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetIsReferenceFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        public GetIsReferenceFactory(TES5ReferenceFactory referenceFactory)
        {
            this.referenceFactory = referenceFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES5Referencer argument = this.referenceFactory.CreateReadReference(functionArguments[0].StringValue, globalScope, multipleScriptsScope, localScope);
#if ALTERNATE_TYPE_MAPPING
            if (!TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(calledOn.TES5Type, TES5BasicType.T_STATIC) && TES5InheritanceGraphAnalyzer.IsTypeOrExtendsType(argument.TES5Type, TES5BasicType.T_STATIC))
            {
                ITES5VariableOrProperty? referencesTo = argument.ReferencesTo;
                if (referencesTo != null) { referencesTo.TES5Type.NativeType = calledOn.TES5Type.NativeType; }
            }
#endif
            TES5ComparisonExpression expression = TES5ExpressionFactory.CreateComparisonExpression(calledOn, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, argument);
            return expression;
        }
    }
}