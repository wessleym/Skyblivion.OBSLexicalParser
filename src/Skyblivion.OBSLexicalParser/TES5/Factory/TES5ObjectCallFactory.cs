using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
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

        public TES5ObjectCall CreateObjectCall(ITES5Referencer callable, string functionName, TES5ObjectCallArguments? arguments = null, bool inference = true)
        {
            TES5ObjectCall objectCall = new TES5ObjectCall(callable, functionName, arguments);
            if (inference)
            {
                this.typeInferencer.InferenceObjectByMethodCall(objectCall);
            }
            return objectCall;
        }

        public TES5ObjectCallCustom CreateObjectCallCustom(ITES5Referencer callable, string functionName, ITES5Type returnType, TES5ObjectCallArguments? arguments = null)
        {
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
            if (globalScope.ScriptHeader.BasicScriptType == TES5BasicType.T_QUEST)
            {
                TES5Property questDelayTimeProperty = globalScope.Properties.Where(p => p.OriginalName.Equals("fquestdelaytime", StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                if (questDelayTimeProperty != null)
                {
                    arguments.Add(TES5ReferenceFactory.CreateReferenceToVariable(questDelayTimeProperty));
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