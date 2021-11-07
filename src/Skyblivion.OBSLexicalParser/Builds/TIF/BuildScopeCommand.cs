using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class BuildScopeCommand : BuildScopeCommandQFOrTIF
    {
        public BuildScopeCommand(TES5PropertyFactory propertyFactory, FragmentsReferencesBuilder fragmentsReferencesBuilder)
            : base(propertyFactory, fragmentsReferencesBuilder, TES5BasicType.T_TOPICINFO, TES5TypeFactory.TES4_Prefix, TES5FragmentType.T_TIF)
        { }
    }
}