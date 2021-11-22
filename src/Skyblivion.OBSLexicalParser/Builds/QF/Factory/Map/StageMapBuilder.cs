using Skyblivion.OBSLexicalParser.TES4.Context;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    static class StageMapBuilder
    {
        public static StageMap Build(IBuildTarget target, string resultingFragmentName, ESMAnalyzer esmAnalyzer, int tes4FormID)
        {
            return
#if USEFILESINSTEADOFESM
                StageMapFromMAPBuilder.BuildStageMap(target, resultingFragmentName)
#else
                StageMapFromESMBuilder.BuildStageMap(esmAnalyzer.GetRecordByFormID(tes4FormID))
#endif
                ;
        }
    }
}
