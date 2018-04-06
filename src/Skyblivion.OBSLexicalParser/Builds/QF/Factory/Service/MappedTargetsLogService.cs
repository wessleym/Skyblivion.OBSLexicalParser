using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service
{
    class MappedTargetsLogService : BuildLogService
    {
        public MappedTargetsLogService(Build build)
            : base(GetPath(build), FileMode.Create)
        { }

        private void WriteLine(string text)
        {
            Write(text + "\r\n");
        }

        public void WriteScriptName(string scriptName)
        {
            WriteLine(scriptName);
        }

        public void WriteLine(int originalTargetIndex, IEnumerable<int> mappedTargetIndexes = null)
        {
            if (mappedTargetIndexes == null) { mappedTargetIndexes = new int[] { }; }
            WriteLine(originalTargetIndex.ToString() + " " + string.Join("\t", mappedTargetIndexes));
        }

        public static string GetPath(Build build)
        {
            return build.GetBuildPath("TargetsMapping.txt");
        }

        public static void DeleteFile(Build build)
        {
            string path = GetPath(build);
            File.Delete(path);
        }
    }
}