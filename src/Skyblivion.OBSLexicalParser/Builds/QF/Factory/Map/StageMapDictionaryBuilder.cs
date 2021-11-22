using System.Collections.Generic;
using System.Globalization;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    static class StageMapDictionaryBuilder
    {
        public static Dictionary<int, List<int>> Build(string[] stageMapFileLines)
        {
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
    }
}
