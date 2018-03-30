using System;
using System.Diagnostics;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    static class ExternalExecution
    {
        public static void Run(string fileName, string arguments, out string standardOutput, out string standardError)
        {
            Console.WriteLine("Executing command: " + fileName + " " + arguments);
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName, arguments);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            string tempStandardError = "";
            using (Process process = Process.Start(startInfo))
            {
                standardOutput = process.StandardOutput.ReadToEnd();
                process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) { tempStandardError += e.Data + "\r\n"; } };
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
            standardError = tempStandardError;
        }

        public static string[] RunPapyrusCompiler(string sourcePath, string workspacePath, string outputPath)
        {
            const string fileName = "mono";
            string compilerPath = Directory.GetCurrentDirectory() + "/Compiler/";
            string arguments = "\"" + compilerPath + "PapyrusCompiler.exe\" \"" + sourcePath + "\" -f=\"" + compilerPath + "/TESV_Papyrus_Flags.flg\" -i=\"" + workspacePath + "\" -o=\"" + outputPath + "\" -a";
            string stdout, stderr;
            ExternalExecution.Run(fileName, arguments, out stdout, out stderr);
            return stderr.Replace("\r\n", "\n").Split('\n');
        }
    }
}