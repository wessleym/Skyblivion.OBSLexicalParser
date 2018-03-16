using System.IO;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    public static class FileTransfer
    {
        public static void CopyDirectoryFiles(string sourceDirectory, string destinationDirectory)
        {
            foreach (string path in Directory.EnumerateFileSystemEntries(sourceDirectory))
            {
                string fileName = Path.GetFileName(path);
                string destinationPath = Path.Combine(destinationDirectory, fileName);
                File.Copy(path, destinationPath);
            }
        }
    }
}
