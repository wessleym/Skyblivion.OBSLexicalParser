using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class StartConversationFactory : IFunctionFactory
    {
        private readonly LogUnknownFunctionFactory logUnknownFunctionFactory;
        public StartConversationFactory(LogUnknownFunctionFactory logUnknownFunctionFactory)
        {
            this.logUnknownFunctionFactory = logUnknownFunctionFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            /*string firstArg = function.Arguments[0].StringValue;
            string md5 = TES5TypeFactory.TES4Prefix + "SCENE_" + PHPFunction.MD5(calledOn.ReferencesTo.ReferenceEDID + firstArg).Substring(0, 16);
            List<string> sceneData = new List<string>() { md5, firstArg };
            if (function.Arguments.Count >= 2)
            {
                sceneData.Add(function.Arguments[1].StringValue);
            }
            this.metadataLogService.WriteLine("ADD_FORCEGREET_SCENE", sceneData);
            ITES5Referencer reference = this.referenceFactory.createReference(md5, globalScope, multipleScriptsScope, codeScope.LocalScope);
            TES5ObjectCallArguments funcArgs = new TES5ObjectCallArguments();*/
            /*
            Force start because in oblivion double using AddScriptPackage would actually overwrite the script package, so we mimic this
            return this.objectCallFactory.createObjectCall(reference, "ForceStart", multipleScriptsScope, funcArgs);
            */
            //WTM:  Note:  Even though I got some parts of LegacyStartConversation to work, I realized that Oblivion calls StartConversation with DIAL.
            //DIAL is not a type in Papyrus.  The DIAL record would have to be converted to a Topic first somehow so StartConversation could be called.
            //I'm going to log calls to this function.
            /*TES4FunctionArguments oldArguments = function.Arguments;
            TES5ObjectCallArguments newArguments = this.objectCallArgumentsFactory.CreateArgumentList(oldArguments, codeScope, globalScope, multipleScriptsScope);
            return this.objectCallFactory.CreateObjectCall(calledOn, "LegacyStartConversation", newArguments);*/
            return logUnknownFunctionFactory.CreateLogCall(function);
        }
    }
}