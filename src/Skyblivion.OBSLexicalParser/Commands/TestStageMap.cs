using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class TestStageMap : LPCommand
    {
        public TestStageMap()
            : base("skyblivion:testStageMap", "Test Stage Map", null)
        { }

        public override void execute()
        {
            Build build = new Build(Build.DEFAULT_BUILD_PATH);
            Dictionary<int, List<int>> originalStageMap;
            using (BuildLogServices buildLogServices = new BuildLogServices(build))
            {
                BuildTarget buildTarget = BuildTargetFactory.get(BuildTarget.BUILD_TARGET_QF, build, buildLogServices);
                originalStageMap = QFFragmentFactory.BuildStageMapDictionary(buildTarget, "QF_FGC01Rats_01035713");
            }
            StageMap stageMap = new StageMap(originalStageMap.ToDictionary(m => m.Key, m => m.Value.ToList()));//Copy dictionary
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

/*
10 - 0 0 0 1 0 0 0
10 - 0 0 0 1 0 0 0
20 - 1 0 0 0 0 0 0
20 - 1 0 0 0 0 0 0
30 - 0 0 0 0 1 0 0
30 - 0 0 0 0 1 0 0
40 - 0 0 1 0 0 0 0
40 - 0 0 1 0 0 0 0
50 - 0 0 1 0 0 0 0
50 - 0 0 1 0 0 0 0
55 - 0 0 0 0 0 0 0
55 - 0 0 0 0 0 0 0
60 - 0 0 0 0 0 1 0
60 - 0 0 0 0 0 1 0
65 - 0 0 0 0 0 0 0
65 - 0 0 0 0 0 0 0
70 - 0 1 0 0 0 0 0
70 - 0 1 0 0 0 0 0
80 - 0 1 0 0 0 0 0
80 - 0 1 0 0 0 0 0
90 - 0 0 0 0 0 0 0
90 - 0 0 0 0 0 0 0
100 - 0 0 0 0 0 0 0
100 - 0 0 0 0 0 0 0
105 - 0 0 0 0 0 0 1
105 - 0 0 0 0 0 0 1
110 - 0 0 0 0 0 0 0
110 - 0 0 0 0 0 0 0
200 - 0 0 0 0 0 0 0
200 - 0 0 0 0 0 0 0
Mapping index print
3 - 4
0 - 5
1 - 6
*/
    }
}