using Skyblivion.ESReader;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.Context.BuildTargetWriters
{
    static class QFReferenceAliasesFileWriter
    {
        public static void Write(string sourcePath, ESRecord qust)
        {
            string[] aliases = ESMAnalyzer.GetReferenceAliases(qust).ToArray();
            string fileNameNoExt = BuildTargetsWriter.GetFileNameNoExt(TES5ReferenceFactory.qf_Prefix, qust, true, null);
            string aliasesPath = sourcePath + fileNameNoExt + ".aliases";
            string aliasesContents = string.Join("\r\n", aliases)
#if !NEWBT
                + "\r\n"
#endif
                ;
            FileSystemEntryUtility.WriteAllTextOrThrowIfExists(aliasesPath, aliasesContents);
        }

        public static void Write()
        {
            ESCollection collection = ESCollectionFactory.CreateForQUSTReferenceAliasExporting(DataDirectory.ESMDirectoryPath, DataDirectory.TES4GameFileName);
            BuildTarget qfBuildTarget = BuildTargetFactory.Construct(BuildTargetFactory.QFName, new Build());
            string sourcePath = qfBuildTarget.GetSourcePath();
            IEnumerable<ESRecord> qustRecords = collection.GetGrupRecords(ESRecordType.QUST);
            foreach (ESRecord qust in qustRecords)
            {
                Write(sourcePath, qust);
            }
        }
    }
}
