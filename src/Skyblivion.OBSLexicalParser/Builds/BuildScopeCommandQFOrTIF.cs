using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildScopeCommandQFOrTIF : IBuildScopeCommand
    {
        private readonly TES5PropertyFactory propertyFactory;
        private readonly FragmentsReferencesBuilder fragmentsReferencesBuilder;
        private readonly TES5BasicType scriptType;
        private readonly string scriptNamePrefix;
        private readonly TES5FragmentType fragmentType;
        public BuildScopeCommandQFOrTIF(TES5PropertyFactory propertyFactory, FragmentsReferencesBuilder fragmentsReferencesBuilder, TES5BasicType scriptType, string scriptNamePrefix, TES5FragmentType fragmentType)
        {
            this.propertyFactory = propertyFactory;
            this.fragmentsReferencesBuilder = fragmentsReferencesBuilder;
            this.scriptType = scriptType;
            this.scriptNamePrefix = scriptNamePrefix;
            this.fragmentType = fragmentType;
        }

        public TES5GlobalScope Build(string sourcePath, TES5GlobalVariables globalVariables)
        {
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath);
            TES4VariableDeclarationList variableList = fragmentsReferencesBuilder.BuildVariableDeclarationList(sourcePath, scriptName, fragmentType);
            //Create the header.
            TES5ScriptHeader scriptHeader = TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(scriptName, scriptType, scriptNamePrefix, true);
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            if (variableList != null)
            {
                propertyFactory.CreateAndAddProperties(variableList, globalScope, globalVariables);
            }
            return globalScope;
        }
    }
}
