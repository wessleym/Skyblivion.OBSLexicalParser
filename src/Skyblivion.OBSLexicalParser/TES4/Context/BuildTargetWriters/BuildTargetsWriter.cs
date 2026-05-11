using Skyblivion.ESReader;

namespace Skyblivion.OBSLexicalParser.TES4.Context.BuildTargetWriters
{
    //This class remakes the BuildTargets folder.
    //When NEWBT (general formatting) and PRESERVECASES are not used as conditional compilation variables, the old BuildTargets will be matched as closely as possible.
    static class BuildTargetsWriter
    {
        public static void Write()
        {
            TES4ScriptWriter.Write();
            QFReferenceAliasesFileWriter.Write();
            ReferencesFileWriter.Write();
            StageMapFromESMWriter.Write();
        }

        public static string GetFileNameNoExt(string prefix, ESRecord record, bool includeFormID, StageIndexAndLogIndex? stageIndexAndLogIndex)
        {
            string? editorID = record.TryGetEditorID();
            string editorID_formID = (editorID != null ? editorID : "")
#if !PRESERVECASES
                .ToLower()
#endif
                + (includeFormID ? "_" + (record.FormID + 0x01000000).ToString("x").PadLeft(8, '0') : "");
            string indexPlusFileNumber = stageIndexAndLogIndex != null ? "_" + stageIndexAndLogIndex.StageIndex.ToString() + "_" + stageIndexAndLogIndex.LogIndex.ToString() : "";
            return prefix + editorID_formID + indexPlusFileNumber;
        }
    }
}
