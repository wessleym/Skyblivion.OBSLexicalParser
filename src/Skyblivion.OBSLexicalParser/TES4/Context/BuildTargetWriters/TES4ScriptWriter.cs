using Skyblivion.ESReader;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.TES4.Context.BuildTargetWriters
{
    static class TES4ScriptWriter
    {
        public static void Write()
        {
            ESCollection collection = ESCollectionFactory.CreateForScriptExporting(DataDirectory.ESMDirectoryPath, DataDirectory.TES4GameFileName);
            string buildTargetsPath = DataDirectory.BuildTargetsPath;
            Write(collection, buildTargetsPath, "Standalone", "", false, null, true, TES4RecordType.SCPT);
            Write(collection, buildTargetsPath, "TIF", TES5ReferenceFactory.TIF_Prefix, true, null, false, ESRecordType.INFO);
            WriteWithIndexes(collection, buildTargetsPath, "QF", TES5ReferenceFactory.QF_Prefix, ESRecordType.QUST);
        }

        private static void Write(ESCollection collection, string buildTargetsPath, string scriptFolderName, ESRecordType recordType, Action<ESRecord, string> writeRecord)
        {
            string directory = buildTargetsPath + scriptFolderName + Path.DirectorySeparatorChar + "Source" + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(directory);
            var records =
                recordType == ESRecordType.INFO ? collection.Where(r => r.RecordType == recordType) :
                collection.GetGrupRecords(recordType);
            foreach (ESRecord record in records)
            {
                writeRecord(record, directory);
            }
        }

        private static void Write(ESCollection collection, string buildTargetsPath, string scriptFolderName, string scriptNamePrefix, bool includeFormID, StageIndexAndLogIndex? stageIndexAndLogIndex, bool allowFixScriptName, ESRecordType recordType)
        {
            Write(collection, buildTargetsPath, scriptFolderName, recordType, (record, directory) =>
            {
                string[] sctxs = record.GetSubrecordsStrings("SCTX").ToArray();
                if (sctxs.Length > 0)
                {
                    if (sctxs.Length > 1) { throw new InvalidOperationException(nameof(sctxs) + ".Length > 1"); }
#if !NEWBT
                    if (recordType == TES4RecordType.SCPT && !ShouldWriteScript(record.GetEditorID())) { return; }
#endif
                    Write(directory, scriptNamePrefix, record, includeFormID, stageIndexAndLogIndex, allowFixScriptName, ESRecord.ReplaceSCTXSpecialCharacters(sctxs[0]));
                }
            });
        }

        private static void WriteWithIndexes(ESCollection collection, string buildTargetsPath, string scriptFolderName, string scriptNamePrefix, ESRecordType recordType)
        {
            Write(collection, buildTargetsPath, scriptFolderName, recordType, (record, directory) =>
            {
                foreach (var subrecord in record.GetSubrecordsWithStageIndexAndLogIndex("SCTX"))
                {
                    string sctx = subrecord.Item1.Value.ToStringTrim();
                    Write(directory, scriptNamePrefix, record, true, subrecord.Item2, false, sctx);
                }
            });
        }

        private static string FixScriptName(ESRecord record, string contents)
        {
            string scriptEditorID = record.GetEditorID();
            Regex soughtScriptName = new Regex("((S|s)cript(N|n)ame|scn)( +|\t)([A-Za-z0-9_]+)( |\t)?(\r\n|$)");
            Match soughtScriptNameMatch = soughtScriptName.Match(contents);
            if (!soughtScriptNameMatch.Success)
            {
                throw new InvalidOperationException("Script name not found in script for " + scriptEditorID + ".");
            }
            Group editorIDGroup = soughtScriptNameMatch.Groups[5];
            if (editorIDGroup.Value != scriptEditorID)
            {
                if (scriptEditorID.Equals(editorIDGroup.Value.Replace("_", ""), StringComparison.OrdinalIgnoreCase))
                {
                    contents = contents.Substring(0, editorIDGroup.Index) + scriptEditorID + contents.Substring(editorIDGroup.Index + editorIDGroup.Length);
                }
                else
                {
                    throw new InvalidOperationException("Script name was invalid but couldn't be replaced.  Expected script name:  " + scriptEditorID + ".");
                }
            }
            return contents;
        }

        private static void Write(string directory, string scriptNamePrefix, ESRecord record, bool includeFormID, StageIndexAndLogIndex? stageIndexAndLogIndex, bool allowFixScriptName, string contents)
        {
            string path = GetPath(directory, scriptNamePrefix, record, includeFormID, stageIndexAndLogIndex);
            if (allowFixScriptName) { contents = FixScriptName(record, contents); }
            contents += scriptNamePrefix != "" ? "\r\n" : "";//only for non-Standalone; allows compilation to complete
            FileSystemEntryUtility.WriteAllTextOrThrowIfExists(path, contents);
        }

        private static string GetPath(string directory, string scriptNamePrefix, ESRecord record, bool includeFormID, StageIndexAndLogIndex? stageIndexAndLogIndex)
        {
            string fileNameNoExt = BuildTargetsWriter.GetFileNameNoExt(scriptNamePrefix, record, includeFormID, stageIndexAndLogIndex);
            string path = directory + fileNameNoExt + ".txt";
            return path;
        }

#if !NEWBT
        private static readonly string[] commentOnlyScripts = new string[] { "DAPeryiteDoorToTamrielSCRIPT", "DarkScalesEffectScript", "DemoMinotaur", "GenericLoreScript", "kmktest", "KurtTestBookScript", "MillonaUmbranoxScript", "SE03DemPuzzleTempSCRIPT", "SE09CeremonyDummySpellScript", "SE10OrderPriestTemplateSCRIPT", "SE11bQuestScript", "SE36QuestScript", "SE40ScrollSpellEffectScript", "SEAlasonScript", "SERelmynaResurrectSpellScript", "sprigscript", "streetlightscript", "testtest" };
        private static bool ShouldWriteScript(string editorID)
        {
            return !commentOnlyScripts.Contains(editorID, StringComparer.OrdinalIgnoreCase);
        }
#endif
    }
}
