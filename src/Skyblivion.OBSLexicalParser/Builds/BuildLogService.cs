using System;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildLogService : IDisposable
    {
        private Lazy<StreamWriter> fileStream;
        protected BuildLogService(string path, bool append)
        {
            fileStream = new Lazy<StreamWriter>(() => new StreamWriter(path, append));
        }

        protected void WriteLine(string text)
        {
            fileStream.Value.WriteLine(text);
        }

        public void Dispose()
        {
            if (fileStream.IsValueCreated) { fileStream.Value.Dispose(); }
        }
    }
}
