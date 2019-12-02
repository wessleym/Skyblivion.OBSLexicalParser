using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.TES5.Factory.Functions
{
    class AddScriptPackageFactory : IFunctionFactory
    {
        private readonly TES5ReferenceFactory referenceFactory;
        private readonly MetadataLogService metadataLogService;
        private readonly TES5ObjectCallFactory objectCallFactory;
        public AddScriptPackageFactory(TES5ReferenceFactory referenceFactory, MetadataLogService metadataLogService, TES5ObjectCallFactory objectCallFactory)
        {
            this.referenceFactory = referenceFactory;
            this.metadataLogService = metadataLogService;
            this.objectCallFactory = objectCallFactory;
        }

        public ITES5ValueCodeChunk ConvertFunction(ITES5Referencer calledOn, TES4Function function, TES5CodeScope codeScope, TES5GlobalScope globalScope, TES5MultipleScriptsScope multipleScriptsScope)
        {
            TES5LocalScope localScope = codeScope.LocalScope;
            TES4FunctionArguments functionArguments = function.Arguments;
            string dataString = functionArguments[0].StringValue;
            string referenceName = NameTransformer.GetEscapedName(calledOn.Name + dataString, TES5TypeFactory.TES4Prefix+"SCENE_", true);
            this.metadataLogService.WriteLine("ADD_SCRIPT_SCENE", new string[] { dataString, referenceName });
            ITES5Referencer reference = this.referenceFactory.CreateReference(referenceName, globalScope, multipleScriptsScope, localScope);
            TES5ObjectCallArguments funcArgs = new TES5ObjectCallArguments();
            /*
             * Force start because in oblivion double using AddScriptPackage would actually overwrite the script package, so we mimic this
             */
            return this.objectCallFactory.CreateObjectCall(reference, "ForceStart", funcArgs);
        }
    }
}