using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using System;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands
{
    class TestStageMap : LPCommand
    {
        protected TestStageMap()
        {
            Name = "skyblivion:testStageMap";
        }

        protected void execute()
        {
            set_time_limit(60);
            Dictionary<int, List<int>>  originalStageMap = this.buildStageMap(BuildTargetFactory.get(BuildTarget.BUILD_TARGET_QF, new Build(Build.DEFAULT_BUILD_PATH)), "QF_FGC01Rats_01035713");
            StageMap stageMap = new StageMap(originalStageMap);
            foreach (var stageId in stageMap.getStageIds())
            {
                Console.WriteLine(stageId.ToString()+" - "+string.Join(" ", originalStageMap[stageId]));
                Console.Write(stageId.ToString()+" - ");
                List<int> map = stageMap.getStageTargetsMap(stageId);
                foreach (var val in map)
                {
                    Console.Write(val+" ");
                }

                Console.WriteLine();
            }

            Console.WriteLine("Mapping index print");
            foreach (var kvp in stageMap.getMappedTargetsIndex())
            {
                var originalTargetIndex = kvp.Key;
                var mappedTargetIndexes = kvp.Value;
                Console.WriteLine(originalTargetIndex+" - "+string.Join(" ", mappedTargetIndexes));
            }
        }

        private Dictionary<int, List<int>> buildStageMap(BuildTarget target, string resultingFragmentName)
        {
            string sourcePath = target.getSourceFromPath(resultingFragmentName);
            string scriptName = Path.GetFileName(sourcePath);
            string stageMapFile = Path.GetDirectoryName(sourcePath)+"/"+scriptName+".map";
            string[] stageMapContent = File.ReadAllLines(stageMapFile);
            Dictionary<int, List<int>> stageMap = new Dictionary<int, List<int>>();
            foreach (var stageMapLine in stageMapContent)
            {
                string[] e = stageMapLine.Split('-');
                int stageId = int.Parse(e[0].Trim());
                /*
                 * Clear the rows
                 */
                string[] stageRowsRaw = e[1].Split(' ');
                List<int> stageRows = new List<int>();
                foreach (var v in stageRowsRaw)
                {
                    string vTrimmed = v.Trim();
                    if (vTrimmed != "")
                    {
                        stageRows.Add(int.Parse(vTrimmed));
                    }
                }

                stageMap[stageId] = stageRows;
            }

            return stageMap;
        }
    }
}