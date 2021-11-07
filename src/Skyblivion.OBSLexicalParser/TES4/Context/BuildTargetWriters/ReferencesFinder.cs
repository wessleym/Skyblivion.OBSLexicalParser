using Dissect.Extensions;
using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.AST.Object;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.Context.BuildTargetWriters
{
    class ReferencesFinder
    {
        public static void Write()
        {
            string buildPath = DataDirectory.GetBuildPath();
            Build build = new Build(buildPath);
            BuildTarget qfBuildTarget = BuildTargetFactory.Construct(BuildTargetFactory.QFName, build);
            BuildTarget tifBuildTarget = BuildTargetFactory.Construct(BuildTargetFactory.TIFName, build);
            TES4Collection collection = TES4CollectionFactory.Create(DataDirectory.GetESMDirectoryPath(), DataDirectory.TES4GameFileName);//Needs the "full" collection for SCRO lookups.
            using (ESMAnalyzer esmAnalyzer = ESMAnalyzer.Load(collection))
            {
                foreach (TES4Record record in collection.Where(r => r.RecordType == TES4RecordType.INFO))
                {
                    var scroRecords = esmAnalyzer.GetTypesFromSCROEnumerable(record, null);
                    string[] aliases = scroRecords.Select(r => r.Key).ToArray();
                    Write(qfBuildTarget, tifBuildTarget, record, null, aliases);
                }
                foreach (TES4Record record in collection.GetGrupRecords(TES4RecordType.QUST))
                {
                    Dictionary<StageIndexAndLogIndex, List<string>> aliasesDictionary = new Dictionary<StageIndexAndLogIndex, List<string>>();
                    foreach (var subrecord in record.GetSubrecordsWithStageIndexAndLogIndex("SCRO"))
                    {
                        int formID = subrecord.Item1.Value.ToInt();
                        string name = formID == TES5PlayerReference.FormID ? TES5PlayerReference.PlayerRefName : esmAnalyzer.GetEDIDByFormID(formID);
                        aliasesDictionary.AddNewListIfNotContainsKeyAndAddValueToList(subrecord.Item2, name);
                    }
                    foreach (var aliases in aliasesDictionary)
                    {
                        Write(qfBuildTarget, tifBuildTarget, record, aliases.Key, aliases.Value);
                    }
                }
            }
        }

        private static void Write(IBuildTarget qfBuildTarget, IBuildTarget tifBuildTarget, TES4Record record, StageIndexAndLogIndex? stageIndexAndLogIndex, IReadOnlyList<string> aliases)
        {
            if (!aliases.Any()) { return; }
#if !NEWBT
            bool playerRefFound = false;
            aliases = aliases.Where(a =>
            {
                if (a == TES5PlayerReference.PlayerRefName) { playerRefFound = true; return false; }
                return true;
            }).ToArray();
#endif
            IBuildTarget buildTarget;
            string prefix;
            GetBuildTargetAndPrefix(qfBuildTarget, tifBuildTarget, record, out buildTarget, out prefix);
            string fileNameNoExt = BuildTargetsWriter.GetFileNameNoExt(prefix, record, true, stageIndexAndLogIndex);
            string aliasesPath = buildTarget.GetSourcePath() + fileNameNoExt + ".references";
            string contents = string.Join("\r\n", aliases)
#if !NEWBT
                + (playerRefFound && aliases.Any() ? "\r\n" : "") + "\r\n\r\n"
#endif
                ;
            FileWriter.WriteAllTextOrThrowIfExists(aliasesPath, contents);
        }

        private static void GetBuildTargetAndPrefix(IBuildTarget qfBuildTarget, IBuildTarget tifBuildTarget, TES4Record record, out IBuildTarget buildTarget, out string prefix)
        {
            if (record.RecordType == TES4RecordType.QUST) { buildTarget = qfBuildTarget; prefix = TES5ReferenceFactory.qf_Prefix; }
            else if (record.RecordType == TES4RecordType.INFO) { buildTarget = tifBuildTarget; prefix = TES5ReferenceFactory.tif_Prefix; }
            else { throw new InvalidOperationException("Invalid record type:  " + record.RecordType.Name); }
        }
    }
}
