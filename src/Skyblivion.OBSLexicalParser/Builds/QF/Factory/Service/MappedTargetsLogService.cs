using System;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service
{
    class MappedTargetsLogService : BuildLogService
    {
        public MappedTargetsLogService(Build build)
            : base(GetPath(build), false)
        { }

        public void WriteScriptName(string scriptName)
        {
            WriteLine(scriptName);
        }

        public void WriteLine(int originalTargetIndex, IEnumerable<int>? mappedTargetIndexes = null)
        {
            if (mappedTargetIndexes == null) { mappedTargetIndexes = Array.Empty<int>(); }
            WriteLine(originalTargetIndex.ToString() + " " + string.Join("\t", mappedTargetIndexes));
        }

        public static string GetPath(Build build)
        {
            return build.CombineWithBuildPath("TargetsMapping.txt");
        }

        public static void DeleteFile(Build build)
        {
            string path = GetPath(build);
            File.Delete(path);
        }
    }
}