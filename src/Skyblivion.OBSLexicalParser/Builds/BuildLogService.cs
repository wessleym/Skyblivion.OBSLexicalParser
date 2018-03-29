using Skyblivion.OBSLexicalParser.Extensions.StreamExtensions;
using System;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildLogService : IDisposable
    {
        private Lazy<FileStream> fileStream;
        protected BuildLogService(string path, FileMode fileMode)
        {
            fileStream = new Lazy<FileStream>(() => new FileStream(path, fileMode));
        }

        protected void Write(string text)
        {
            fileStream.Value.WriteUTF8(text);
        }

        public void Dispose()
        {
            if (fileStream.IsValueCreated) { fileStream.Value.Dispose(); }
        }
    }
}
