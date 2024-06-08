using Skyblivion.OBSLexicalParser.Data;
using System;
using System.Diagnostics;
using System.IO;

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

        public static void Run(string sourceDirectoryOrFilePath, string importPath, string outputPath, string standardOutputFilePath, string standardErrorFilePath)
        {
            //https://ck.uesp.net/wiki/Papyrus_Compiler_Reference
            //PapyrusCompiler seems to require that paths don't start with . and that only forward slashes are used.
            bool file = sourceDirectoryOrFilePath.EndsWith(".psc", StringComparison.OrdinalIgnoreCase);
            sourceDirectoryOrFilePath = sourceDirectoryOrFilePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            importPath = importPath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            outputPath = outputPath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            standardOutputFilePath = standardOutputFilePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            string compilerDirectory = DataDirectory.CompilerDirectoryPath;
            string compilerPath = compilerDirectory + "PapyrusCompiler.exe";
            string flagsPath = compilerDirectory.Replace("\\", "/") + "TESV_Papyrus_Flags.flg";
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            string fileName = (isWindows ? "\"" : "") + "." + Path.DirectorySeparatorChar + compilerPath + (isWindows ? "\"" : "");
            string arguments =
                "\"" + sourceDirectoryOrFilePath + "\"" +
                " -flags=\"" + flagsPath + "\"" +
                " -import=\"" + importPath + "\"" +
                " -output=\"" + outputPath + "\"" +
                (!file ? " -all" : "");
            Console.WriteLine("Executing PapyrusCompiler.exe:  " + fileName + " " + arguments);
            ProcessStart(fileName, arguments, standardOutputFilePath, standardErrorFilePath);
            Console.WriteLine("PapyrusCompiler.exe Complete");
        }

        public static string FixReferenceName(string referenceName)
        {
            //Papyrus compiler somehow treats properties with "temp" in them in a special way, so we change them to tmp to accommodate that.
            //WTM:  Note:  In the original PHP version, this was case sensitive.  I'm leaving it as such.
            return referenceName.Replace("temp", "tmp");
        }
    }
}