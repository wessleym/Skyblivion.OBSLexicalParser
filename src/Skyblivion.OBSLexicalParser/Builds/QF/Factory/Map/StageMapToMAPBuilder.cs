using System;
using System.Collections.Generic;
using System.Text;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map
{
    class StageMapToMAPBuilder
    {
        public static string GetContents(StageMap stageMap, Dictionary<int, List<int>> originalStageMap)
        {
            StringBuilder output = new StringBuilder();
            foreach (var stageId in stageMap.StageIDs)
            {
                string stageIDString = stageId.ToString();
                output.Append(stageIDString).Append(" - ").AppendLine(string.Join(" ", originalStageMap[stageId]));
                output.Append(stageIDString).Append(" -");
                List<int> map = stageMap.GetStageTargetsMap(stageId);
                foreach (var val in map)
                {
                    output.Append(" ").Append(val);
                }
                output.AppendLine();
            }
            return output.ToString();
        }
    }
}
