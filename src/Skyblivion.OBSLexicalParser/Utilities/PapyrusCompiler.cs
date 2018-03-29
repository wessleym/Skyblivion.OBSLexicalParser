using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    static class PapyrusCompiler
    {
        private static void ProcessStartNonWindows(string fileName, string arguments, string standardOutputFilePath, string standardErrorFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, arguments);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            string standardOutput, standardError = "";
            using (Process process = Process.Start(startInfo))
            {
                process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) { standardError += e.Data + "\r\n"; } };
                process.BeginErrorReadLine();
                process.WaitForExit();
                standardOutput = process.StandardOutput.ReadToEnd();
            }
            File.WriteAllText(standardOutputFilePath, standardOutput);
            if (standardError != "")
            {
                File.WriteAllText(standardErrorFilePath, standardError);
            }
        }

        private static Regex lfWithoutCR = new Regex("(?<!\r)\n");
        private static void ProcessStartWindows(string fileName, string arguments, string standardOutputFilePath, string standardErrorFilePath)
        {//This method uses an alternate technique for getting standard output and standard error.
            //PapyrusCompiler.exe stops running after a few seconds if RedirectStandardOutput or RedirectStandardError are true.
            //Also, calling PapyrusCompiler.exe directly while using >> and 2> causes problems.  So I'm calling cmd.
            string newArguments = "/s /c \"" + fileName + " " + arguments + " >>\"" + standardOutputFilePath + "\" 2>>\"" + standardErrorFilePath + "\"\"";
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd", newArguments);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
            //PapyrusCompiler sometimes writes \n only.
            File.WriteAllText(standardOutputFilePath, lfWithoutCR.Replace(File.ReadAllText(standardOutputFilePath), "\r\n"));
            File.WriteAllText(standardErrorFilePath, lfWithoutCR.Replace(File.ReadAllText(standardErrorFilePath), "\r\n"));
        }

        public static void Run(string sourcePath, string workspacePath, string outputPath, string standardOutputFilePath, string standardErrorFilePath)
        {
            //PapyrusCompiler seems to require that paths don't start with . and that only forward slashes are used.
            sourcePath = sourcePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            workspacePath = workspacePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            outputPath = outputPath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            standardOutputFilePath = standardOutputFilePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            string compilerDirectory = "Compiler" + Path.DirectorySeparatorChar;
            string fileName = "\"" + compilerDirectory + "PapyrusCompiler.exe\"";
            string arguments = "\"" + sourcePath + "\" -f=\"" + compilerDirectory + "TESV_Papyrus_Flags.flg\" -i=\"" + workspacePath + "\" -o=\"" + outputPath + "\" -a";
            Console.WriteLine("Executing PapyrusCompiler.exe:  " + fileName + " " + arguments);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                ProcessStartWindows(fileName, arguments, standardOutputFilePath, standardErrorFilePath);
            }
            else
            {
                ProcessStartNonWindows(fileName, arguments, standardOutputFilePath, standardErrorFilePath);
            }
            Console.WriteLine("PapyrusCompiler.exe Complete");
        }

        public static string FixReferenceName(string referenceName)
        {
            //Papyrus compiler somehow treats properties with "temp" in them in a special way, so we change them to tmp to accomodate that.
            return Regex.Replace(referenceName, "temp", "tmp");
        }
    }
}