using Skyblivion.ESReader;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.Utilities;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.TES4.Context.BuildTargetWriters
{
    static class StageMapFromESMWriter
    {
        public static void Write()
        {
            BuildTarget qfBuildTarget = BuildTargetFactory.Construct(BuildTargetFactory.QFName, new Build());
            string sourcePath = qfBuildTarget.GetSourcePath();
            ESCollection collection = ESCollectionFactory.CreateForQUSTStageMapExportingFromESM(DataDirectory.ESMDirectoryPath, DataDirectory.TES4GameFileName);
            IEnumerable<ESRecord> qusts = collection.GetGrupRecords(ESRecordType.QUST);
            foreach (ESRecord qust in qusts)
            {
                string fileNameNoExt = BuildTargetsWriter.GetFileNameNoExt(TES5ReferenceFactory.QF_Prefix, qust, true, null);
                string fileName = fileNameNoExt + ".map";
                string mapFilePath = sourcePath + Path.DirectorySeparatorChar + fileName;
                string contentsString = StageMapFromESMBuilder.BuildString(qust);
                FileSystemEntryUtility.WriteAllTextOrThrowIfExists(mapFilePath, contentsString);
            }
        }
    }
}
