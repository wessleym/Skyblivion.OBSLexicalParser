using System;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    public static class FileTransfer
    {
        public static void CopyDirectoryFiles(string sourceDirectory, string destinationDirectory, bool overwrite)
        {
            if (!sourceDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                throw new ArgumentException("Invalid Directory", nameof(sourceDirectory));
            }
            if (!destinationDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                throw new ArgumentException("Invalid Directory", nameof(destinationDirectory));
            }
            Directory.CreateDirectory(destinationDirectory);
            foreach (string path in Directory.EnumerateFiles(sourceDirectory))
            {
                string fileName = Path.GetFileName(path);
                string destinationPath = destinationDirectory + fileName;
                File.Copy(path, destinationPath, overwrite);
            }
            foreach (string sourceSubDirectoryPath in Directory.EnumerateDirectories(sourceDirectory))
            {
                DirectoryInfo sourceSubDirectory = new DirectoryInfo(sourceSubDirectoryPath);
                string destinationSubDirectoryPath = Path.Combine(destinationDirectory, sourceSubDirectory.Name) + Path.DirectorySeparatorChar;
                CopyDirectoryFiles(sourceSubDirectoryPath + Path.DirectorySeparatorChar, destinationSubDirectoryPath, overwrite);
            }
        }
    }
}
