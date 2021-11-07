using Skyblivion.ESReader.TES4;
using System.IO;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES4.Context.BuildTargetWriters
{
    //This class remakes the BuildTargets folder.  When NEWBT is not used as a conditional compilation variable, the old BuildTargets will be matched as closely as possible.
    static class BuildTargetsWriter
    {
        public static void Write()
        {
            TES4ScriptWriter.Write();
            QFReferenceAliasFinder.Write();
            ReferencesFinder.Write();
            StageMapFromPSCBuilder.Write();
       }

        public static string GetFileNameNoExt(string prefix, TES4Record record, bool includeFormID, StageIndexAndLogIndex? stageIndexAndLogIndex)
        {
            string? editorID = record.TryGetEditorID();
            string editorID_formID = (editorID != null ? editorID : "").ToLower() + (includeFormID ? "_" + (record.FormID + 0x01000000).ToString("x").PadLeft(8, '0') : "");
            string indexPlusFileNumber = stageIndexAndLogIndex != null ? "_" + stageIndexAndLogIndex.StageIndex.ToString() + "_" + stageIndexAndLogIndex.LogIndex.ToString() : "";
            return prefix + editorID_formID + indexPlusFileNumber;
        }

        //This removes certain text so these files can be more easily compared with previous build targets.
        private static void RemoveTextForComparisons()
        {
            Regex nextFragmentIndexRE = new Regex(";NEXT FRAGMENT INDEX [0-9]+", RegexOptions.Compiled);
            foreach (string path in Directory.EnumerateFiles(@"C:\Users\Wess\Documents\Visual Studio 2019\Projects\Skyblivion.OBSLexicalParser\Skyblivion.OBSLexicalParserApp\bin\Debug\net5.0\Data\Build\Transpiled\", "*.*", SearchOption.AllDirectories))
            {
                string contents = File.ReadAllText(path);
                contents = nextFragmentIndexRE.Replace(contents, "");
                File.WriteAllText(path, contents);
            }
        }
    }
}
