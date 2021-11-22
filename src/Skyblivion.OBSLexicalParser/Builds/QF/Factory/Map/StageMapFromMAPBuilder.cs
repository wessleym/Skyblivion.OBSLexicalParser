using Skyblivion.ESReader.Extensions;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    static class StageMapFromMAPBuilder
    {
        public static Dictionary<int, List<int>> BuildStageMapDictionary(IBuildTarget target, string resultingFragmentName, out bool preprocessed)
        {
            string[] stageMapFileLines = GetStageMapFileLines(target, resultingFragmentName, out preprocessed);
            return StageMapDictionaryBuilder.Build(stageMapFileLines);
        }

        private static string[] GetStageMapFileLines(IBuildTarget target, string resultingFragmentName, out bool preprocessed)
        {
            preprocessed = false;
            string sourcePath = target.GetSourceFromPath(resultingFragmentName);
            //ToLower() is needed for Linux's case-sensitive file system since these files seem to all be lowercase.
            string scriptName = Path.GetFileNameWithoutExtension(sourcePath).ToLower();
            string? sourceDirectory = Path.GetDirectoryName(sourcePath);
            if (sourceDirectory == null) { throw new NullableException(nameof(sourceDirectory)); }
            string stageMapFile = Path.Combine(sourceDirectory, scriptName + ".map");
            string[] stageMapFileLines;
            try
            {
                stageMapFileLines = File.ReadAllLines(stageMapFile);
            }
            catch (IOException)
            {
                preprocessed = true;
                stageMapFile = Path.Combine(sourceDirectory, scriptName + ".map2");
                stageMapFileLines = File.ReadAllLines(stageMapFile);
            }
            return stageMapFileLines;
        }

        public static StageMap BuildStageMap(IBuildTarget target, string resultingFragmentName)
        {
            bool preprocessed;
            Dictionary<int, List<int>> stageMap = BuildStageMapDictionary(target, resultingFragmentName, out preprocessed);
            return new StageMap(stageMap, preprocessed);
        }
    }
}
