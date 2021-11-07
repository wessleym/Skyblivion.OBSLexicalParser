using Skyblivion.ESReader.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    static class StageMapFromMAPBuilder
    {
        public static Dictionary<int, List<int>> BuildStageMapDictionary(IBuildTarget target, string resultingFragmentName, out bool preprocessed)
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
            Dictionary<int, List<int>> stageMap = new Dictionary<int, List<int>>();
            foreach (var stageMapLine in stageMapFileLines)
            {
                string[] numberAndItemsSplit = stageMapLine.Split('-');
                int stageId = int.Parse(numberAndItemsSplit[0].Trim(), CultureInfo.InvariantCulture);
                /*
                 * Clear the rows
                 */
                string[] items = numberAndItemsSplit[1].Split(' ');
                List<int> stageRows = new List<int>();
                foreach (string item in items)
                {
                    string itemTrimmed = item.Trim();
                    if (itemTrimmed != "")
                    {
                        stageRows.Add(int.Parse(itemTrimmed, CultureInfo.InvariantCulture));
                    }
                }

                stageMap.Add(stageId, stageRows);
            }

            return stageMap;
        }

        public static StageMap BuildStageMap(IBuildTarget target, string resultingFragmentName)
        {
            bool preprocessed;
            Dictionary<int, List<int>> stageMap = BuildStageMapDictionary(target, resultingFragmentName, out preprocessed);
            return new StageMap(stageMap, preprocessed);
        }
    }
}
