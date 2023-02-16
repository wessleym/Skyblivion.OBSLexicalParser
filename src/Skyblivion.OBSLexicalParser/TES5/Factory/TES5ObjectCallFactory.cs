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
using System.Linq;

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
        public TES5ObjectCall CreateObjectCall(ITES5Referencer callable, TES4Function function, string newFunctionName, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return CreateObjectCall(callable, newFunctionName, arguments: objectCallArgumentsFactory.CreateArgumentList(function.Arguments, codeScope, globalScope, multipleScriptsScope), comment: function.Comment);
        }
        public TES5ObjectCall CreateObjectCall(ITES5Referencer callable, TES4Function function, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return CreateObjectCall(callable, function, function.FunctionCall.FunctionName, objectCallArgumentsFactory, codeScope, globalScope, multipleScriptsScope);
        }

        public TES5ObjectCallCustom CreateObjectCallCustom(ITES5Referencer callable, string functionName, ITES5Type returnType, TES5ObjectCallArguments? arguments = null)
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

        public TES5ObjectCall CreateRegisterForSingleUpdate(TES5GlobalScope globalScope)
        {
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments();
            if (globalScope.ScriptHeader.ScriptType.NativeType == TES5BasicType.T_QUEST)
            {
                TES5Property? questDelayTimeProperty = globalScope.Properties.Where(p => p.OriginalName.Equals("fquestdelaytime", StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                if (questDelayTimeProperty != null)
                {
                    arguments.Add(TES5ReferenceFactory.CreateReferenceToVariableOrProperty(questDelayTimeProperty));
                }
                else
                {
                    arguments.Add(new TES5Float(5));
                }
            }
            else
            {
                arguments.Add(new TES5Float(0.4f));
            }
            return CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "RegisterForSingleUpdate", arguments);
        }

        public TES5ObjectCall CreateGotoState(string stateName, TES5GlobalScope globalScope)
        {
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { new TES5String(stateName) };
            return CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "GotoState", arguments);
        }
    }
}