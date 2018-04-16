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
        private ESMAnalyzer esmAnalyzer;
        private StandaloneParsingService standaloneParsingService;
        public BuildScopeCommand(StandaloneParsingService standaloneParsing)
        {
            this.standaloneParsingService = standaloneParsing;
        }

        public void initialize()
        {
            this.esmAnalyzer = new ESMAnalyzer(DataDirectory.TES4GameFileName);
        }
        
        private TES5ScriptHeader createHeader(TES4Script script)
        {
            string edid = script.ScriptHeader.ScriptName;
            return new TES5ScriptHeader(edid, this.esmAnalyzer.getScriptType(edid), TES5TypeFactory.TES4Prefix);
        }

        public TES5GlobalScope buildScope(string scriptPath, TES5GlobalVariables globalVariables)
        {
            TES4Script parsedScript = this.standaloneParsingService.parseScript(scriptPath);
            TES5ScriptHeader scriptHeader = this.createHeader(parsedScript);
            TES4VariableDeclarationList variableList = parsedScript.VariableDeclarationList;
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            if (variableList != null)
            {
                TES5PropertiesFactory.createProperties(variableList, globalScope, globalVariables);
            }
            return globalScope;
        }
    }
}