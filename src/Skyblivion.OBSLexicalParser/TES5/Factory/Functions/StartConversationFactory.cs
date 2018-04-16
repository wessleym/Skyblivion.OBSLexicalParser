using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class StartConversationFactory : IFunctionFactory
    {
        private TES5ReferenceFactory referenceFactory;
        private TES5VariableAssignationFactory assignationFactory;
        private ESMAnalyzer analyzer;
        private TES5ObjectPropertyFactory objectPropertyFactory;
        private TES5TypeInferencer typeInferencer;
        private MetadataLogService metadataLogService;
        private TES5ValueFactory valueFactory;
        private TES5ObjectCallFactory objectCallFactory;
        private TES5ObjectCallArgumentsFactory objectCallArgumentsFactory;
        public StartConversationFactory(TES5ValueFactory valueFactory, TES5ObjectCallFactory objectCallFactory, TES5ObjectCallArgumentsFactory objectCallArgumentsFactory, TES5ReferenceFactory referenceFactory, TES5VariableAssignationFactory assignationFactory, TES5ObjectPropertyFactory objectPropertyFactory, ESMAnalyzer analyzer,TES5TypeInferencer typeInferencer, MetadataLogService metadataLogService)
        {
            this.objectCallArgumentsFactory = objectCallArgumentsFactory;
            this.valueFactory = valueFactory;
            this.referenceFactory = referenceFactory;
            this.analyzer = analyzer;
            this.assignationFactory = assignationFactory;
            this.objectPropertyFactory = objectPropertyFactory;
            this.typeInferencer = typeInferencer;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk convertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
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
            return this.objectCallFactory.createObjectCall(reference, "ForceStart",multipleScriptsScope, funcArgs);
            */

            return new TES5Filler();
        }
    }
}