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
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, arguments);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            string standardOutput, standardError = "";
            using (Process process = Process.Start(startInfo))
            {
                process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) { standardError += e.Data + Environment.NewLine; } };
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

        private static Regex lfWithoutCR = new Regex("(?<!\r)\n", RegexOptions.Compiled);
        private static void ProcessStartAlternate(string fileName, string arguments, string standardOutputFilePath, string standardErrorFilePath)
        {//This method uses an alternate technique for getting standard output and standard error.
            //PapyrusCompiler.exe stops running after a few seconds if RedirectStandardOutput or RedirectStandardError are true.
            //Also, calling PapyrusCompiler.exe directly while using >> and 2> causes problems.  So I'm calling cmd or bash.
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            string newFileName = isWindows ? "cmd" : "bash";
            string quotationMark = isWindows ? "\"" : "'";
            string cmdOptions = isWindows ? "/s /c" : "-c";
            string stdOutRedirector = isWindows ? ">>" : ">";
            string stdErrRedirector = isWindows ? "2>>" : "2>";
            string newArguments = cmdOptions + " " + quotationMark + fileName + " " + arguments + " "+ stdOutRedirector+"\"" + standardOutputFilePath + "\" "+ stdErrRedirector + "\"" + standardErrorFilePath + "\"" + quotationMark;
            ProcessStartInfo startInfo = new ProcessStartInfo(newFileName, newArguments);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
            string standardOutputText = File.ReadAllText(standardOutputFilePath);
            string standardErrorText = File.ReadAllText(standardErrorFilePath);
            if (isWindows)
            {
                //PapyrusCompiler sometimes writes \n only.
                standardOutputText = lfWithoutCR.Replace(standardOutputText, Environment.NewLine);
                standardErrorText = lfWithoutCR.Replace(standardErrorText, Environment.NewLine);
            }
            File.WriteAllText(standardOutputFilePath, standardOutputText);
            File.WriteAllText(standardErrorFilePath, standardErrorText);
        }

        public static void Run(string sourcePath, string workspacePath, string outputPath, string standardOutputFilePath, string standardErrorFilePath)
        {
            //PapyrusCompiler seems to require that paths don't start with . and that only forward slashes are used.
            sourcePath = sourcePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            workspacePath = workspacePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            outputPath = outputPath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            standardOutputFilePath = standardOutputFilePath.Trim('.', Path.DirectorySeparatorChar).Replace("\\", "/");
            string compilerDirectory = DataDirectory.GetCompilerDirectoryPath();
            bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
            string fileName = (isWindows ? "\"" : "") + "." + Path.DirectorySeparatorChar + compilerDirectory + "PapyrusCompiler.exe" + (isWindows ? "\"" : "");
            string arguments = "\"" + sourcePath + "\" -f=\"" + compilerDirectory + "TESV_Papyrus_Flags.flg\" -i=\"" + workspacePath + "\" -o=\"" + outputPath + "\" -a";
            Console.WriteLine("Executing PapyrusCompiler.exe:  " + fileName + " " + arguments);
            ProcessStartAlternate(fileName, arguments, standardOutputFilePath, standardErrorFilePath);
            Console.WriteLine("PapyrusCompiler.exe Complete");
        }

        public static string FixReferenceName(string referenceName)
        {
            //Papyrus compiler somehow treats properties with "temp" in them in a special way, so we change them to tmp to accomodate that.
            return Regex.Replace(referenceName, "temp", "tmp");
        }
    }
}