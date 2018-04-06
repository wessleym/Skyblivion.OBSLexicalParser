using Skyblivion.OBSLexicalParser.Builds;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.TES5.Service
{
    class MetadataLogService : BuildLogService
    {
        public MetadataLogService(Build build)
            : base(GetPath(build), FileMode.Append)
        { }

        public void WriteLine(string command, IEnumerable<string> arguments = null)
        {
            if (arguments == null) { arguments = new string[] { }; }
            Write(command + " " + string.Join("\t", arguments) + "\r\n");
        }

        private static string GetPath(Build build)
        {
            return build.GetBuildPath("Metadata.txt");
        }

        public static void DeleteFile(Build build)
        {
            string path = GetPath(build);
            File.Delete(path);
        }
    }
}