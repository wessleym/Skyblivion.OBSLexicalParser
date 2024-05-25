using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetSecondsPassedFactory : IFunctionFactory
    {
        private readonly LogUnknownFunctionFactory logUnknownFunctionFactory;
        public GetSecondsPassedFactory(LogUnknownFunctionFactory logUnknownFunctionFactory)
        {
            this.logUnknownFunctionFactory = logUnknownFunctionFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //First Attempt:
            /*
            TES5LocalScope localScope = codeScope.LocalScope;
            ITES5Referencer timerReference = this.referenceFactory.CreateTimerReadReference(globalScope, multipleScriptsScope, localScope);
            ITES5Referencer localTimeReference = this.referenceFactory.CreateGSPLocalTimerReadReference(globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments methodArguments = new TES5ObjectCallArguments() { calledOn };
            return this.objectCallFactory.CreateObjectCall(timerReference, "getSecondsPassed", multipleScriptsScope, methodArguments);*/
            //const string newFunctionName = "GetSecondsPassed";
            //Second Attempt:
            /*TES5BasicType returnType = TES5BasicType.T_FLOAT;
            globalScope.AddFunctionIfNotExists(newFunctionName, () =>
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
            //Third attempt:
            //return this.objectCallFactory.CreateObjectCall(calledOn, newFunctionName, comment: function.Comment);
            return logUnknownFunctionFactory.CreateReplacement(function, new TES5Integer(1000), codeScope);
        }
    }
}