using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class ModPCInfamyFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public ModPCInfamyFactory(TES5ReferenceFactory referenceFactory, TES5ObjectCallFactory objectCallFactory)
        {
            this.referenceFactory = referenceFactory;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            ITES5Referencer fameReference = this.referenceFactory.CreateReadReference("Infamy", globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments fameArguments = new TES5ObjectCallArguments();
            TES5ArithmeticExpression arithmeticExpression = TES5ExpressionFactory.CreateArithmeticExpression(fameReference, TES5ArithmeticExpressionOperator.OPERATOR_ADD, new TES5Integer(((TES4Integer)functionArguments[0]).IntValue));
            fameArguments.Add(arithmeticExpression);
            TES5ObjectCall newFunction = this.objectCallFactory.CreateObjectCall(this.referenceFactory.CreateReference("Infamy", globalScope, multipleScriptsScope, localScope), "SetValue", fameArguments, comment: function.Comment);
            return newFunction;
        }
    }
}