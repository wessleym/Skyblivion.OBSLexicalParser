using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;
using System.IO;
using System.Linq;

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

        public TES5GlobalScope Build(string sourcePath, TES5GlobalVariableCollection globalVariables)
        {
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath);
            TES4VariableDeclaration[] variableDeclarations = fragmentsReferencesBuilder.BuildVariableDeclarationList(sourcePath, scriptName, fragmentType).ToArray();
            //Create the header.
            TES5ScriptHeader scriptHeader = TES5ScriptHeaderFactory.GetFromCacheOrConstructByBasicType(scriptName, scriptType, scriptNamePrefix, true);
            TES5GlobalScope globalScope = new TES5GlobalScope(scriptHeader);
            propertyFactory.CreateAndAddProperties(variableDeclarations, globalScope, globalVariables);
            return globalScope;
        }
    }
}
