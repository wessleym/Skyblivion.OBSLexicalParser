using Skyblivion.ESReader.TES4;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.Context.BuildTargetWriters
{
    static class QFReferenceAliasFinder
    {
        public static void Write(string sourcePath, TES4Record qust)
        {
            string[] aliases = ESMAnalyzer.GetReferenceAliases(qust).ToArray();
            string fileNameNoExt = BuildTargetsWriter.GetFileNameNoExt(TES5ReferenceFactory.qf_Prefix, qust, true, null);
            string aliasesPath = sourcePath + fileNameNoExt + ".aliases";
            string aliasesContents = string.Join("\r\n", aliases)
#if !NEWBT
                + "\r\n"
#endif
                ;
            FileWriter.WriteAllTextOrThrowIfExists(aliasesPath, aliasesContents);
        }

        public static void Write()
        {
            TES4Collection collection = TES4CollectionFactory.CreateForQUSTReferenceAliasExporting(DataDirectory.GetESMDirectoryPath(), DataDirectory.TES4GameFileName);
            BuildTarget qfBuildTarget = BuildTargetFactory.Construct(BuildTargetFactory.QFName, new Build(DataDirectory.GetBuildPath()));
            string sourcePath = qfBuildTarget.GetSourcePath();
            IEnumerable<TES4Record> qustRecords = collection.GetGrupRecords(TES4RecordType.QUST);
            foreach (TES4Record qust in qustRecords)
            {
                Write(sourcePath, qust);
            }
        }
    }
}
