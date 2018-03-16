using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Extensions.StreamExtensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.TES5.Service
{
    class MetadataLogService : IDisposable
    {
        private FileStream stream;
        public MetadataLogService(Build build)
        {
            string path = build.getBuildPath()+"Metadata";
            stream = new FileStream(path, FileMode.Append);
        }

        public void add(string command, IEnumerable<string> arguments = null)
        {
            if (arguments == null) { arguments = new string[] { }; }
            stream.WriteUTF8(command + " " + string.Join("\t", arguments) + "\r\n");
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}