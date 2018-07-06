using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Block;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code.Branch;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression;
using Skyblivion.OBSLexicalParser.TES5.AST.Expression.Operators;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetSecondsPassedFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly ESMAnalyzer analyzer;
        private readonly TES5ObjectPropertyFactory objectPropertyFactory;
        private readonly TES5TypeInferencer typeInferencer;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ValueFactory valueFactory;
        private readonly TES5ObjectCallFactory objectCallFactory;
        private readonly TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public GetSecondsPassedFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
            this.objectPropertyFactory = objectPropertyFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        /*public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            ITES5Referencer timerReference = this.referenceFactory.CreateTimerReadReference(globalScope, multipleScriptsScope, localScope);
            ITES5Referencer localTimeReference = this.referenceFactory.CreateGSPLocalTimerReadReference(globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments methodArguments = new TES5ObjectCallArguments() { calledOn };
            return this.objectCallFactory.CreateObjectCall(timerReference, "getSecondsPassed", multipleScriptsScope, methodArguments);
        }*/
        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            const string newFunctionName = "GetSecondsPassed";
            TES5BasicType returnType = TES5BasicType.T_FLOAT;
            /*globalScope.AddFunctionIfNotExists(newFunctionName, () =>
            {
                const string getCurrentRealTime = "GetCurrentRealTime";
                TES5FunctionScope newFunctionScope = new TES5FunctionScope(newFunctionName);
                TES5LocalScope newLocalScope = TES5LocalScopeFactory.CreateRootScope(newFunctionScope);
                TES5CodeScope newCodeScope = TES5CodeScopeFactory.CreateCodeScope(newLocalScope);
                TES5GlobalVariable calledYet = new TES5GlobalVariable("GSPCalledYet");
                TES5GlobalVariable lastSeconds = new TES5GlobalVariable("GSPLastSeconds");
                TES5SubBranch ifNotCalledYet = TES5BranchFactory.CreateSubBranch(new TES5ComparisonExpression(calledYet, TES5ComparisonExpressionOperator.OPERATOR_EQUAL, new TES5Bool(false)), newLocalScope);
                ifNotCalledYet.CodeScope.AddChunk(TES5VariableAssignationFactory.CreateAssignation(calledYet, new TES5Bool(true)));
                ifNotCalledYet.CodeScope.AddChunk(TES5VariableAssignationFactory.CreateAssignation(lastSeconds, this.objectCallFactory.CreateObjectCall(TES5StaticReference.Utility, getCurrentRealTime, multipleScriptsScope)));
                ifNotCalledYet.CodeScope.AddChunk(new TES5Return(new TES5Float(0)));
                TES5CodeScope ifCalledYetCodeScope = TES5CodeScopeFactory.CreateCodeScopeRoot(newFunctionScope);
                ifCalledYetCodeScope.AddChunk(new TES5Return(new TES5ArithmeticExpression(this.objectCallFactory.CreateObjectCall(TES5StaticReference.Utility, getCurrentRealTime, multipleScriptsScope), TES5ArithmeticExpressionOperator.OPERATOR_SUBSTRACT, lastSeconds)));
                TES5ElseSubBranch ifCalledYet = new TES5ElseSubBranch(ifCalledYetCodeScope);
                TES5Branch branch = new TES5Branch(ifNotCalledYet, elseBranch: ifCalledYet);
                TES5FunctionCodeBlock getSecondsPassedLocalFunction = new TES5FunctionCodeBlock(newFunctionScope, newCodeScope, returnType, true);
                getSecondsPassedLocalFunction.AddChunk(branch);
                return getSecondsPassedLocalFunction;
            });*/
            return this.objectCallFactory.CreateObjectCall(calledOn, newFunctionName, multipleScriptsScope);
        }
    }
}