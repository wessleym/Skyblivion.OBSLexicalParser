using Skyblivion.OBSLexicalParser.TES4.AST;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Context;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.Builds.Service;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.Data;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class BuildScopeCommand : IBuildScopeCommand
    {
        const string SCRIPTS_PREFIX = "TES4";
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
            string edid = script.getScriptHeader().getScriptName();
            string scriptName = TES5NameTransformer.TransformLongName(edid, SCRIPTS_PREFIX);
            return new TES5ScriptHeader(scriptName, edid, this.esmAnalyzer.getScriptType(edid), SCRIPTS_PREFIX);
        }

        public TES5GlobalScope buildScope(string scriptPath, TES5GlobalVariables globalVariables)
        {
            TES4Script parsedScript = this.standaloneParsingService.parseScript(scriptPath);
            TES5ScriptHeader scriptHeader = this.createHeader(parsedScript);
            TES4VariableDeclarationList variableList = parsedScript.getVariableDeclarationList();
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            if (variableList != null)
            {
                TES5PropertiesFactory.createProperties(variableList, globalScope, globalVariables);
            }
            return globalScope;
        }
    }
}