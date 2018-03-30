using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service
{
    class MappedTargetsLogService : BuildLogService
    {
        public MappedTargetsLogService(Build build)
            : base(build.GetBuildPath("TargetsMapping"), FileMode.Create)
        { }

        private void WriteLine(string text)
        {
            Write(text + "\r\n");
        }

        public void writeScriptName(string scriptName)
        {
            WriteLine(scriptName);
        }

        public void add(int originalTargetIndex, IEnumerable<int> mappedTargetIndexes = null)
        {
            if (mappedTargetIndexes == null) { mappedTargetIndexes = new int[] { }; }
            WriteLine(originalTargetIndex.ToString() + " " + string.Join("\t", mappedTargetIndexes));
        }
    }
}