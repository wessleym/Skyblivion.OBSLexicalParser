using System;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    public static class FileSystemEntryUtility
    {
        public static void CopyDirectoryFiles(string sourceDirectory, string destinationDirectory, bool overwrite, string? progressWriterLabel = null)
        {
            ProgressWriter? progressWriter = progressWriterLabel != null ? new ProgressWriter(progressWriterLabel, Directory.EnumerateFiles(sourceDirectory).Count()) : null;
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
                if (progressWriter != null) { progressWriter.IncrementAndWrite(); }
            }
            if (progressWriter != null) { progressWriter.WriteLast(); }
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

        public static string? ReadAllTextIfExists(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (FileNotFoundException) { return null; }
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

        public static void DeleteDirectoryIfExists(string path)
        {
            try { Directory.Delete(path, true); }
            catch (DirectoryNotFoundException) { }
        }

        public static void CreateDirectoryEmpty(string path)
        {
            DeleteDirectoryIfExists(path);
            Directory.CreateDirectory(path);
        }

        public static void MoveOverwrite(string source, string destination)
        {
#if NET
            File.Move(source, destination, true);
#else
            try
            {
                File.Move(source, destination);
            }
            catch (IOException)
            {
                File.Delete(destination);
                File.Move(source, destination);
            }
#endif
        }

        public static void MoveOrCopyFiles(string sourceDirectory, string destinationDirectory, bool copy)
        {
            foreach (var sourcePath in Directory.GetFiles(sourceDirectory))
            {
                string destinationPath = destinationDirectory + Path.GetFileName(sourcePath);
                if (copy)
                {
                    File.Copy(sourcePath, destinationPath);
                }
                else
                {
                    File.Move(sourcePath, destinationPath);
                }
            }
        }
    }
}
