using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class TestStageMap : LPCommand
    {
        public TestStageMap()
            : base("skyblivion:testStageMap", "Test Stage Map", null)
        { }

        public override void Execute()
        {
            if (!PreExecutionChecks(false, false, false, false)) { return; }
            Build build = new Build(Build.DEFAULT_BUILD_PATH);
            Dictionary<int, List<int>> originalStageMap;
            using (BuildLogServices buildLogServices = new BuildLogServices(build))
            {
                BuildTarget buildTarget = BuildTargetFactory.Get(BuildTarget.BUILD_TARGET_QF, build, buildLogServices);
                originalStageMap = QFFragmentFactory.BuildStageMapDictionary(buildTarget, "QF_FGC01Rats_01035713");
            }
            StageMap stageMap = new StageMap(originalStageMap.ToDictionary(m => m.Key, m => m.Value.ToList()));//Copy dictionary
            StringBuilder output = new StringBuilder();
            foreach (var stageId in stageMap.StageIDs)
            {
                output.AppendLine(stageId.ToString()+" - "+string.Join(" ", originalStageMap[stageId]));
                output.Append(stageId.ToString()+" -");
                List<int> map = stageMap.GetStageTargetsMap(stageId);
                foreach (var val in map)
                {
                    output.Append(" " + val);
                }

                output.AppendLine();
            }

            output.Append("Mapping index print");
            foreach (var kvp in stageMap.MappedTargetsIndex)
            {
                var originalTargetIndex = kvp.Key;
                var mappedTargetIndexes = kvp.Value;
                output.AppendLine();
                output.Append(originalTargetIndex+" - "+string.Join(" ", mappedTargetIndexes));
            }

            string outputString = output.ToString();
            Console.WriteLine(outputString);
            const string fgc01RatsResultFromPHP =
@"10 - 0 0 0 1
10 - 0 0 0 1 0 0 0 0 0 0
20 - 1 0 0 0
20 - 1 0 0 0 0 0 0 0 0 0
30 - 0 0 0 1
30 - 0 0 0 0 1 0 0 0 0 0
40 - 0 0 1 0
40 - 0 0 1 0 0 0 0 0 0 0
50 - 0 0 1 0
50 - 0 0 1 0 0 0 0 0 0 0
55 - 0 0 0 1
55 - 0 0 0 0 0 1 0 0 0 0
60 - 1 0 0 0
60 - 0 0 0 0 0 0 1 0 0 0
65 - 0 0 0 1
65 - 0 0 0 0 0 0 0 1 0 0
70 - 0 1 0 0
70 - 0 1 0 0 0 0 0 0 0 0
80 - 0 1 0 0
80 - 0 1 0 0 0 0 0 0 0 0
90 - 0 0 0 1
90 - 0 0 0 0 0 0 0 0 1 0
100 - 0 0 0 0
100 - 0 0 0 0 0 0 0 0 0 0
105 - 0 1 0 0
105 - 0 0 0 0 0 0 0 0 0 1
110 - 0 0 0 0
110 - 0 0 0 0 0 0 0 0 0 0
200 - 0 0 0 0
200 - 0 0 0 0 0 0 0 0 0 0
Mapping index print
3 - 4 5 7 8
0 - 6
1 - 9";
            bool match = fgc01RatsResultFromPHP.Replace("\r\n", "\n") == outputString.Replace("\r\n", "\n");
            Console.WriteLine("Output " + (match ? "matched" : "did not match") + " the output of the PHP version.");
        }
    }
}