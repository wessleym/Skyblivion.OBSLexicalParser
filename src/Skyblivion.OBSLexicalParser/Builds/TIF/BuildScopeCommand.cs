using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class BuildScopeCommand : IBuildScopeCommand
    {
        private readonly TES5PropertyFactory propertyFactory;
        public BuildScopeCommand(TES5PropertyFactory propertyFactory)
        {
            this.propertyFactory = propertyFactory;
        }

        public TES5GlobalScope Build(string sourcePath, TES5GlobalVariables globalVariables)
        {
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath);
            string referencesPath = Path.Combine(Path.GetDirectoryName(sourcePath), scriptName + ".references");
            //Create the header.
            TES5ScriptHeader scriptHeader = TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(scriptName, TES5BasicType.T_TOPICINFO, "", true);
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            TES4VariableDeclarationList variableList = FragmentsReferencesBuilder.BuildVariableDeclarationList(referencesPath);
            if (variableList != null)
            {
                propertyFactory.CreateProperties(variableList, globalScope, globalVariables);
            }
            return globalScope;
        }
    }
}