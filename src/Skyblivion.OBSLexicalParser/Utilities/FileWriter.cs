using System;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    public static class FileWriter
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
        }

        public static void CopyDirectoryFilesRecursive(string sourceDirectory, string destinationDirectory, bool overwrite)
        {
            CopyDirectoryFiles(sourceDirectory, destinationDirectory, overwrite);
            foreach (string sourceSubDirectoryPath in Directory.EnumerateDirectories(sourceDirectory))
            {
                string destinationSubDirectoryPath = Path.Combine(destinationDirectory, Path.GetFileName(sourceSubDirectoryPath)) + Path.DirectorySeparatorChar;
                CopyDirectoryFilesRecursive(sourceSubDirectoryPath + Path.DirectorySeparatorChar, destinationSubDirectoryPath, overwrite);
            }
        }

        public static void WriteAllTextOrThrowIfExists(string filePath, string contents)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.CreateNew))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(contents);
                }
            }
        }

        public static void WriteAllTextIfNotExists(string filePath, string contents)
        {
            try
            {
                WriteAllTextOrThrowIfExists(filePath, contents);
            }
            catch (IOException) { }
        }
    }
}
