using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class BuildScopeCommand : IBuildScopeCommand
    {
        private readonly StandaloneParsingService standaloneParsingService;
        private readonly ESMAnalyzer esmAnalyzer;
        private readonly TES5PropertyFactory propertyFactory;
        public BuildScopeCommand(StandaloneParsingService standaloneParsingService, ESMAnalyzer esmAnalyzer, TES5PropertyFactory propertyFactory)
        {
            this.standaloneParsingService = standaloneParsingService;
            this.esmAnalyzer = esmAnalyzer;
            this.propertyFactory = propertyFactory;
        }
        
        private TES5ScriptHeader CreateHeader(TES4Script script)
        {
            string edid = script.ScriptHeader.ScriptName;
            ITES5Type type = this.esmAnalyzer.GetScriptTypeByScriptNameFromCache(edid);
            return TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(edid, type, TES5TypeFactory.TES4Prefix, false);
        }

        public TES5GlobalScope Build(string scriptPath, TES5GlobalVariableCollection globalVariables)
        {
            TES4Script parsedScript = this.standaloneParsingService.ParseOrGetFromCache(scriptPath);
            TES5ScriptHeader scriptHeader = this.CreateHeader(parsedScript);
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            propertyFactory.CreateAndAddProperties(parsedScript.VariableDeclarationsAndComments, globalScope, globalVariables);
            return globalScope;
        }
    }
}