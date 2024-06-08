using Skyblivion.ESReader.TES4;
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
            TES4Collection collection = TES4CollectionFactory.CreateForQUSTStageMapExportingFromESM(DataDirectory.ESMDirectoryPath, DataDirectory.TES4GameFileName);
            IEnumerable<TES4Record> qusts = collection.GetGrupRecords(TES4RecordType.QUST);
            foreach (TES4Record qust in qusts)
            {
                string fileNameNoExt = BuildTargetsWriter.GetFileNameNoExt(TES5ReferenceFactory.qf_Prefix, qust, true, null);
                string fileName = fileNameNoExt + ".map";
                string mapFilePath = sourcePath + Path.DirectorySeparatorChar + fileName;
                string contentsString = StageMapFromESMBuilder.BuildString(qust);
                FileWriter.WriteAllTextOrThrowIfExists(mapFilePath, contentsString);
            }
        }
    }
}
