using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Other;
using Skyblivion.OBSLexicalParser.TES5.Types;

namespace Skyblivion.OBSLexicalParser.Builds.QF
{
    class BuildScopeCommand : BuildScopeCommandQFOrTIF
    {
        public BuildScopeCommand(TES5PropertyFactory propertyFactory, FragmentsReferencesBuilder fragmentsReferencesBuilder)
            : base(propertyFactory, fragmentsReferencesBuilder, TES5BasicType.T_QUEST, "", TES5FragmentType.T_QF)
        { }
    }
}