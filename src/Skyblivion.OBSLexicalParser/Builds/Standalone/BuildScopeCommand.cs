using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class BuildScopeCommand : IBuildScopeCommand
    {
        private readonly ESMAnalyzer esmAnalyzer;
        private readonly StandaloneParsingService standaloneParsingService;
        public BuildScopeCommand(StandaloneParsingService standaloneParsing, bool loadESMAnalyzerLazily)
        {
            this.standaloneParsingService = standaloneParsing;
            this.esmAnalyzer = new ESMAnalyzer(loadESMAnalyzerLazily, DataDirectory.TES4GameFileName);
        }
        
        private TES5ScriptHeader CreateHeader(TES4Script script)
        {
            string edid = script.ScriptHeader.ScriptName;
            return new TES5ScriptHeader(edid, this.esmAnalyzer.GetScriptType(edid), TES5TypeFactory.TES4Prefix);
        }

        public TES5GlobalScope Build(string scriptPath, TES5GlobalVariables globalVariables)
        {
            TES4Script parsedScript = this.standaloneParsingService.ParseOrGetFromCache(scriptPath);
            TES5ScriptHeader scriptHeader = this.CreateHeader(parsedScript);
            TES4VariableDeclarationList variableList = parsedScript.VariableDeclarationList;
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            if (variableList != null)
            {
                TES5PropertiesFactory.CreateProperties(variableList, globalScope, globalVariables);
            }
            return globalScope;
        }
    }
}