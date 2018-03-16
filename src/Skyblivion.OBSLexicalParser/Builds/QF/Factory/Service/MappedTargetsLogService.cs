using Skyblivion.OBSLexicalParser.Extensions.StreamExtensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.QF.Factory.Service
{
    class MappedTargetsLogService : IDisposable
    {
        private FileStream stream;
        public MappedTargetsLogService(Build build)
        {
            string filename = build.getBuildPath()+"TargetsMapping";
            this.stream = new FileStream(filename, FileMode.Create);
        }

        private void WriteLine(string text)
        {
            stream.WriteUTF8(text + "\r\n");
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

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}