using Skyblivion.OBSLexicalParser.Builds;
using System;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.TES5.Service
{
    class MetadataLogService : BuildLogService
    {
        public MetadataLogService(Build build)
            : base(GetPath(build), false)
        { }

        public void WriteLine(string command, IEnumerable<string>? arguments = null)
        {
            if (arguments == null) { arguments = Array.Empty<string>(); }
            base.WriteLine(command + " " + string.Join("\t", arguments));
        }

        private static string GetPath(Build build)
        {
            return build.CombineWithBuildPath("Metadata.txt");
        }

        public static void DeleteFile(Build build)
        {
            string path = GetPath(build);
            File.Delete(path);
        }
    }
}