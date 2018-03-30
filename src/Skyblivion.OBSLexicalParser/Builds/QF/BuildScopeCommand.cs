using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.IO;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;

namespace Skyblivion.OBSLexicalParser.Builds.QF
{
    class BuildScopeCommand : IBuildScopeCommand
    {
        private TES5NameTransformer nameTransformer;
        private FragmentsReferencesBuilder fragmentsReferencesBuilder;
        private TES5PropertiesFactory propertiesFactory;
        public void initialize()
        {
            this.nameTransformer = new TES5NameTransformer();
            this.fragmentsReferencesBuilder = new FragmentsReferencesBuilder();
            this.propertiesFactory = new TES5PropertiesFactory();
        }

        public TES5GlobalScope buildScope(string sourcePath, TES5GlobalVariables globalVariables)
        {
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath);
            string referencesPath = Path.Combine(Path.GetDirectoryName(sourcePath), scriptName + ".references");
            //Create the header.
            TES5ScriptHeader scriptHeader = new TES5ScriptHeader(TES5NameTransformer.transform(scriptName, ""), scriptName, TES5BasicType.T_QUEST, "", true);
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            TES4VariableDeclarationList variableList = this.fragmentsReferencesBuilder.buildVariableDeclarationList(referencesPath);
            if (variableList != null)
            {
                this.propertiesFactory.createProperties(variableList, globalScope, globalVariables);
            }

            return globalScope;
        }
    }
}