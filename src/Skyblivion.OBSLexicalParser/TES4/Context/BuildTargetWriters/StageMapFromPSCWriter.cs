using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES4.Context.BuildTargetWriters
{
    //This is likely no longer necessary thanks to StageMapFromESMWriter.
    static class StageMapFromPSCWriter
    {
        private readonly static Regex fragmentRE = new Regex(@"Function Fragment_([0-9]+)(_[0-9]+)?\(\)\r\n(.*?)\r\nEndFunction", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly static Regex isObjectiveDisplayedRE = new Regex(@"If \(\(__temp\.IsObjectiveDisplayed\(([0-9]+)\) == (0|1)\)\)", RegexOptions.Compiled);
        public static Dictionary<int, List<int>> BuildStageMapDictionaryFromFile(string pscPath)
        {
            Dictionary<int, List<int>> stageMap = new Dictionary<int, List<int>>();
            string psc = File.ReadAllText(pscPath);
            MatchCollection functions = fragmentRE.Matches(psc);
            foreach (Match function in functions)
            {
                List<int> line = new List<int>();
                int stageID = int.Parse(function.Groups[1].Value);
                string functionBody = function.Groups[3].Value;
                MatchCollection isObjectiveDisplayeds = isObjectiveDisplayedRE.Matches(functionBody);
                int expectedIndex = 0;
                foreach (Match isObjectiveDisplayed in isObjectiveDisplayeds)
                {
                    int index = int.Parse(isObjectiveDisplayed.Groups[1].Value);
                    int value = int.Parse(isObjectiveDisplayed.Groups[2].Value);
                    line.Add(value == 1 ? 0 : 1);
                    expectedIndex++;
                }
                List<int> existingLine;
                if (stageMap.TryGetValue(stageID, out existingLine))
                {
                    if (!existingLine.SequenceEqual(line)) { throw new InvalidOperationException(nameof(existingLine) + " did not match " + nameof(line) + ":\r\n" + nameof(existingLine) + ":  " + string.Join(" ", existingLine) + "\r\n"+nameof(line)+":  " + string.Join(" ", line)); }
                }
                else { stageMap.Add(stageID, line); }
            }
            return stageMap;
        }

        private static void Write(string sourcePath, string pscPath)
        {
            string fileNameNoExt = Path.GetFileNameWithoutExtension(pscPath);
            string newFileNameNoExt;
            if (fileNameNoExt == TES5TypeFactory.TES4_Prefix + "1bf7d09a7523fede20dd74f0c224167e") { newFileNameNoExt = "qf_bladesmartinconvsystem_01067dbc"; }
            else if (fileNameNoExt == TES5TypeFactory.TES4_Prefix + "894694c5f17fb6e2afc84ed1b5da2776") { newFileNameNoExt = "qf_imperiallegiondialogue_0118b11c"; }
            else if (fileNameNoExt == TES5TypeFactory.TES4_Prefix + "89e50284d52a5ae8b066499bc8ff2829") { newFileNameNoExt = "qf_arenaspectatorcombatant_0101e691"; }
            else { newFileNameNoExt = fileNameNoExt.Replace(TES5TypeFactory.TES4_Prefix, "").ToLower(); }
            string mapPath = sourcePath + newFileNameNoExt + ".map2";
            string mapContents = string.Join("\r\n", BuildStageMapDictionaryFromFile(pscPath).Select(l => l.Key + " - " + string.Join(" ", l.Value)));
            FileWriter.WriteAllTextOrThrowIfExists(mapPath, mapContents);
        }

        public static void Write()
        {
            BuildTarget qfBuildTarget = BuildTargetFactory.Construct(BuildTargetFactory.QFName, new Build());
            string transpiledPath = qfBuildTarget.GetTranspiledPath();
            string sourcePath = qfBuildTarget.GetSourcePath();
            foreach (string pscPath in Directory.EnumerateFiles(transpiledPath, "*.psc"))
            {
                Write(sourcePath, pscPath);
            }
            TES4Collection collection = TES4CollectionFactory.CreateForQUSTStageMapExportingFromPSCFiles(DataDirectory.ESMDirectoryPath, DataDirectory.TES4GameFileName);
            IEnumerable<TES4Record> qustRecords = collection.GetGrupRecords(TES4RecordType.QUST);
            foreach (TES4Record qust in qustRecords)
            {
                string path = sourcePath + BuildTargetsWriter.GetFileNameNoExt(TES5ReferenceFactory.qf_Prefix, qust, true, null) + ".map2";
                FileWriter.WriteAllTextIfNotExists(path, "");
            }
        }
    }
}
