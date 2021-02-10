using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class BuildScopeCommand : IBuildScopeCommand
    {
        private readonly TES5PropertyFactory propertyFactory;
        private readonly FragmentsReferencesBuilder fragmentsReferencesBuilder;
        public BuildScopeCommand(TES5PropertyFactory propertyFactory, FragmentsReferencesBuilder fragmentsReferencesBuilder)
        {
            this.propertyFactory = propertyFactory;
            this.fragmentsReferencesBuilder = fragmentsReferencesBuilder;
        }

        public TES5GlobalScope Build(string sourcePath, TES5GlobalVariables globalVariables)
        {
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath);
            string? sourceDirectory = Path.GetDirectoryName(sourcePath);
            if (sourceDirectory == null) { throw new NullableException(nameof(sourceDirectory)); }
            string referencesPath = Path.Combine(sourceDirectory, scriptName + ".references");
            //Create the header.
            TES5ScriptHeader scriptHeader = TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(scriptName, TES5BasicType.T_TOPICINFO, TES5TypeFactory.TES4_Prefix, true);
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            TES4VariableDeclarationList variableList = fragmentsReferencesBuilder.BuildVariableDeclarationList(referencesPath, TES5FragmentType.T_TIF);
            if (variableList != null)
            {
                propertyFactory.CreateAndAddProperties(variableList, globalScope, globalVariables);
            }
            return globalScope;
        }
    }
}