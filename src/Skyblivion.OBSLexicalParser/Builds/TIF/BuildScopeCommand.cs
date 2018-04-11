using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using Skyblivion.OBSLexicalParser.Utilities;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class BuildScopeCommand : IBuildScopeCommand
    {
        public void initialize()
        { }

        public TES5GlobalScope buildScope(string sourcePath, TES5GlobalVariables globalVariables)
        {
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath);
            string referencesPath = Path.Combine(Path.GetDirectoryName(sourcePath), scriptName + ".references");
            //Create the header.
            TES5ScriptHeader scriptHeader = new TES5ScriptHeader(scriptName, TES5BasicType.T_TOPICINFO, "", true);
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            TES4VariableDeclarationList variableList = FragmentsReferencesBuilder.buildVariableDeclarationList(referencesPath);
            if (variableList != null)
            {
                TES5PropertiesFactory.createProperties(variableList, globalScope, globalVariables);
            }
            return globalScope;
        }
    }
}