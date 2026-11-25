using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class GetSecondsPassedFactory : IFunctionFactory
    {
        private readonly TES5ObjectCallFactory objectCallFactory;
        public GetSecondsPassedFactory(TES5ObjectCallFactory objectCallFactory)
        {
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5CodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            //First Attempt:
            /*TES5LocalScope localScope = codeScope.LocalScope;
            ITES5Referencer timerReference = this.referenceFactory.CreateTimerReadReference(globalScope, multipleScriptsScope, localScope);
            ITES5Referencer localTimeReference = this.referenceFactory.CreateGSPLocalTimerReadReference(globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments methodArguments = new TES5ObjectCallArguments() { calledOn };
            return this.objectCallFactory.CreateObjectCall(timerReference, "getSecondsPassed", multipleScriptsScope, methodArguments);*/
            //Second Attempt:
            /*const string newFunctionName = "GetSecondsPassed";
            TES5BasicType returnType = TES5BasicType.T_FLOAT;
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
            const string secondsPassedName = "secondsPassed";
            TES5LocalScope parentScope = codeScope.LocalScope.GetUltimateParentScope();
            ITES5LocalVariable? secondsPassedExisting = parentScope.TryGetVariable(secondsPassedName);
            if (secondsPassedExisting != null) { return TES5ReferenceFactory.CreateReferenceToVariableOrProperty(secondsPassedExisting); }
            //For ActiveMagicEffect, calledOn will have been transformed into GetTargetActor() by this point.
            TES5BasicType scriptBaseType = TES5InheritanceGraphAnalyzer.GetUltimateBaseClass(globalScope.ScriptHeader.ScriptType.NativeType);
            ITES5Referencer fixedCalledOn = scriptBaseType == TES5BasicType.T_ACTIVEMAGICEFFECT ? TES5ReferenceFactory.CreateReferenceToSelf(globalScope) : calledOn;
            string functionName =
                scriptBaseType == TES5BasicType.T_FORM ? "GetSecondsPassed" :
                scriptBaseType == TES5BasicType.T_ACTIVEMAGICEFFECT ? "GetSecondsPassedByActiveEffect" :
                throw new ConversionException("Unknown GetSecondsPassed Type: " + scriptBaseType.Name);
            TES5ObjectCall getSecondsPassed = this.objectCallFactory.CreateObjectCall(TES5StaticReferenceFactory.Create(TES5BasicType.T_SKYBFormUtility), functionName, arguments: [fixedCalledOn], comment: function.Comment);
            TES5Reference secondsPassed = TES5LocalVariableFactory.CreateWithInitialization(TES5BasicType.T_FLOAT, secondsPassedName, getSecondsPassed, parentScope);
            return secondsPassed;
            //Fourth attempt:
            //return logUnknownFunctionFactory.CreateReplacement(calledOn, function, new TES5Integer(1000), codeScope, globalScope);
            //Fifth attempt:
            /*TES5Property? questDelayTimeProperty;
            Nullable<float> timerInterval = globalScope.GetTimerIntervalIfNoQuestDelayTime(out questDelayTimeProperty);
            if (timerInterval != null)
            {
                return TES5ReferenceFactory.CreateTimerInterval(codeScope, timerInterval.Value);
            }
            return TES5ReferenceFactory.CreateReferenceToVariableOrProperty(questDelayTimeProperty!);*/
        }
    }
}