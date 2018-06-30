using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ObjectCallFactory
    {
        private TES5TypeInferencer typeInferencer;
        public TES5ObjectCallFactory(TES5TypeInferencer typeInferencer)
        {
            this.typeInferencer = typeInferencer;
        }

        public TES5ObjectCall CreateObjectCall(ITES5Referencer callable, string functionName, TES5MultipleScriptsScope multipleScriptsScope, TES5ObjectCallArguments arguments = null, bool inference = true)
        {
            TES5ObjectCall objectCall = new TES5ObjectCall(callable, functionName, arguments);
            if (inference)
            {
                this.typeInferencer.inferenceObjectByMethodCall(objectCall, multipleScriptsScope);
            }
            return objectCall;
        }

        public TES5ObjectCall CreateGetActorBase(ITES5Referencer calledOn, TES5MultipleScriptsScope multipleScriptsScope)
        {
            return CreateObjectCall(calledOn, "GetActorBase", multipleScriptsScope);
        }

        public TES5ObjectCall CreateGetActorBaseOfPlayer(TES5MultipleScriptsScope multipleScriptsScope)
        {
            return CreateGetActorBase(TES5ReferenceFactory.CreateReferenceToPlayer(), multipleScriptsScope);
        }

        public TES5ObjectCall CreateRegisterForSingleUpdate(TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            float interval = globalScope.ScriptHeader.BasicScriptType == TES5BasicType.T_QUEST ? 5 : 1;
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { new TES5Float(interval) };
            return CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "RegisterForSingleUpdate", multipleScriptsScope, arguments);
        }

        public TES5ObjectCall CreateGotoState(string stateName, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5ObjectCallArguments arguments = new TES5ObjectCallArguments() { new TES5String(stateName) };
            return CreateObjectCall(TES5ReferenceFactory.CreateReferenceToSelf(globalScope), "GotoState", multipleScriptsScope, arguments);
        }
    }
}