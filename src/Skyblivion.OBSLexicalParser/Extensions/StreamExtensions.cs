using System.IO;
using System.Text;

namespace Skyblivion.OBSLexicalParser.Extensions.StreamExtensions
{
    public static class StreamExtensions
    {
        public static void WriteUTF8(this Stream stream, string text)
        {
            byte[] bytes=Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
