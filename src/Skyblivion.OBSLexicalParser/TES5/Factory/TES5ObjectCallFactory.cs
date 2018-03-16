using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    class TES5ObjectCallFactory
    {
        private TES5TypeInferencer typeInferencer;
        public TES5ObjectCallFactory(TES5TypeInferencer typeInferencer)
        {
            this.typeInferencer = typeInferencer;
        }

        public TES5ObjectCall createObjectCall(ITES5Referencer callable, string functionName, TES5MultipleScriptsScope multipleScriptsScope, TES5ObjectCallArguments arguments = null, bool inference = true)
        {
            TES5ObjectCall objectCall = new TES5ObjectCall(callable, functionName, arguments);
            if (inference)
            {
                this.typeInferencer.inferenceObjectByMethodCall(objectCall, multipleScriptsScope);
            }
            return objectCall;
        }
    }
}