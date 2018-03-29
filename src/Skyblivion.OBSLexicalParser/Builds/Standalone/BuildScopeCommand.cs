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

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class BuildScopeCommand : Skyblivion.OBSLexicalParser.Builds.IBuildScopeCommand
    {
        const string SCRIPTS_PREFIX = "TES4";
        private ESMAnalyzer esmAnalyzer;
        private TES5NameTransformer nameTransformer;
        private TES5PropertiesFactory propertiesFactory;
        private StandaloneParsingService standaloneParsingService;
        public BuildScopeCommand(StandaloneParsingService standaloneParsing)
        {
            this.standaloneParsingService = standaloneParsing;
        }

        public void initialize()
        {
            TypeMapper typeMapper = new TypeMapper();
            this.esmAnalyzer = new ESMAnalyzer(typeMapper, "Oblivion.esm");
            this.nameTransformer = new TES5NameTransformer();
            this.propertiesFactory = new TES5PropertiesFactory();
        }
        
        private TES5ScriptHeader createHeader(TES4Script script)
        {
            string edid = script.getScriptHeader().getScriptName();
            string scriptName = TES5NameTransformer.transform(edid, SCRIPTS_PREFIX);
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
                this.propertiesFactory.createProperties(variableList, globalScope, globalVariables);
            }
            return globalScope;
        }
    }
}