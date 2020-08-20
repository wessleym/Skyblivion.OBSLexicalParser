using Skyblivion.OBSLexicalParser.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    static class PapyrusCompiler
    {
        private static void ProcessStart(string fileName, string arguments, string standardOutputFilePath, string standardErrorFilePath)
        {
            using (StreamWriter standardOutput = new StreamWriter(standardOutputFilePath, true))
            {
                using (StreamWriter standardError = new StreamWriter(standardErrorFilePath, true))
                {
                    using (Process process = new Process()
                    {
                        StartInfo = new ProcessStartInfo(fileName, arguments)
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        }
                    })
                    {
                        process.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) { standardOutput.WriteLine(e.Data); } };
                        process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) { standardError.WriteLine(e.Data); } };
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    }
                }
            }
        }

        public static void Run(string sourcePath, string importPath, string outputPath, string standardOutputFilePath, string standardErrorFilePath)
        {
            //PapyrusCompiler seems to require that paths don't start with . and that only forward slashes are used.
            sourcePath = sourcePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            importPath = importPath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            outputPath = outputPath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            standardOutputFilePath = standardOutputFilePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            string compilerDirectory = DataDirectory.GetCompilerDirectoryPath();
            string compilerPath = compilerDirectory + "PapyrusCompiler.exe";
            string flagsPath = compilerDirectory.Replace("\\", "/") + "TESV_Papyrus_Flags.flg";
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            string fileName = (isWindows ? "\"" : "") + "." + Path.DirectorySeparatorChar + compilerPath + (isWindows ? "\"" : "");
            string arguments = "\"" + sourcePath + "\" -f=\"" + flagsPath + "\" -i=\"" + importPath + "\" -o=\"" + outputPath + "\" -a";
            Console.WriteLine("Executing PapyrusCompiler.exe:  " + fileName + " " + arguments);
            ProcessStart(fileName, arguments, standardOutputFilePath, standardErrorFilePath);
            Console.WriteLine("PapyrusCompiler.exe Complete");
        }

        public static string FixReferenceName(string referenceName)
        {
            //Papyrus compiler somehow treats properties with "temp" in them in a special way, so we change them to tmp to accommodate that.
            return Regex.Replace(referenceName, "temp", "tmp");
        }

        public static string UnfixReferenceName(string referenceName)
        {
            return Regex.Replace(referenceName, "tmp", "temp");
        }
    }
}