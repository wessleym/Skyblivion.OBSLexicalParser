using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Factory.Functions;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ObjectCallFactory
    {
        private readonly TES5TypeInferencer typeInferencer;
        public TES5ObjectCallFactory(TES5TypeInferencer typeInferencer)
        {
            this.typeInferencer = typeInferencer;
        }

        public TES5ObjectCall CreateObjectCall(ITES5Referencer callable, string functionName, TES5ObjectCallArguments? arguments = null, TES4Comment? comment = null, bool inference = true)
        {
            if (arguments == null) { arguments = new TES5ObjectCallArguments(); }
            TES5Comment? tes5Comment = comment != null ? TES5CommentFactory.Construct(comment) : null;
            TES5ObjectCall objectCall = new TES5ObjectCall(callable, functionName, arguments, tes5Comment);
            if (inference)
            {
                this.typeInferencer.InferenceObjectByMethodCall(objectCall);
            }
            return objectCall;
        }
        public TES5ObjectCall CreateObjectCall(ITES5Referencer callable, TES4Function functionForNameAndComments, TES5ObjectCallArguments? arguments = null)
        {
            return CreateObjectCall(callable, functionForNameAndComments.FunctionCall.FunctionName, arguments: arguments, comment: functionForNameAndComments.Comment);
        }
        public TES5ObjectCall CreateObjectCall(ITES5Referencer callable, TES4Function functionForArgumentsAndComments, string newFunctionName, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return CreateObjectCall(callable, newFunctionName, arguments: objectCallArgumentsFactory.CreateArgumentList(functionForArgumentsAndComments.Arguments, codeScope, globalScope, multipleScriptsScope), comment: functionForArgumentsAndComments.Comment);
        }
        public TES5ObjectCall CreateObjectCall(ITES5Referencer callable, TES4Function function, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return CreateObjectCall(callable, function, function.FunctionCall.FunctionName, objectCallArgumentsFactory, codeScope, globalScope, multipleScriptsScope);
        }

        public static TES5ObjectCallCustom CreateObjectCallCustom(ITES5Referencer callable, string functionName, ITES5Type returnType, TES5ObjectCallArguments? arguments = null)
        {
            if (arguments == null) { arguments = new TES5ObjectCallArguments(); }
            return new TES5ObjectCallCustom(callable, functionName, returnType, arguments);
        }

        public TES5ObjectCall CreateGetActorBase(ITES5Referencer calledOn)
        {
            return CreateObjectCall(calledOn, "GetActorBase");
        }

        public TES5ObjectCall CreateGetActorBaseOfPlayer(TES5GlobalScope globalScope)
        {
            return CreateGetActorBase(TES5ReferenceFactory.CreateReferenceToPlayer(globalScope));
        }

        public TES5ObjectCall CreateRegisterForSingleUpdate(TES5CodeScope codeScope, TES5GlobalScope globalScope)
        {
            TES5Property? questDelayTimeProperty;
            Nullable<float> timerInterval = globalScope.GetTimerIntervalIfNoQuestDelayTime(out questDelayTimeProperty);
            TES5Reference argument = timerInterval != null ?
                TES5ReferenceFactory.CreateTimerInterval(codeScope, timerInterval.Value) :
                TES5ReferenceFactory.CreateReferenceToVariableOrProperty(questDelayTimeProperty!);
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments()
            {
                argument
            };
            return CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "RegisterForSingleUpdate", arguments);
        }

        public TES5ObjectCall CreateGotoState(string stateName, TES5GlobalScope globalScope)
        {
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { new TES5String(stateName) };
            return CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "GotoState", arguments);
        }
    }
}