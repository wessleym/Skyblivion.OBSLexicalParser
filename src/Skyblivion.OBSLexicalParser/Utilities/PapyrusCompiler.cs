using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Utilities
{
    static class PapyrusCompiler
    {
        private static void ProcessStart(string fileName, string arguments, string standardOutputFilePath, string standardErrorFilePath, Action<string>? onNewOutput = null)
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
                        process.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) { standardOutput.WriteLine(e.Data); if (onNewOutput != null) { onNewOutput(e.Data); } } };
                        process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) { standardError.WriteLine(e.Data); } };
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();
                    }
                }
            }
        }

        private static int GetCompileFileCount(string sourceDirectoryOrFilePath)
        {
            return Directory.Exists(sourceDirectoryOrFilePath) ? Directory.EnumerateFiles(sourceDirectoryOrFilePath, "*.psc").Count() : 1;
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
            int compileTotal = GetCompileFileCount(sourceDirectoryOrFilePath);
            Console.WriteLine("Executing PapyrusCompiler.exe:  " + fileName + " " + arguments);
            ProgressWriter progressWriter = new ProgressWriter("Compiling Files", compileTotal);
            ProcessStart(fileName, arguments, standardOutputFilePath, standardErrorFilePath, onNewOutput: output =>
            {
                //The compiler exe outputs several lines at once, some of which are not complete lines.
                //It's possible the string I'm seeking might start in one chunk and end in another chunk, meaning I am not counting everything.
                //For 9634 files, 9628 were counted.
                int newlyComplete = output.Split(new string[] { "Compilation succeeded." }, StringSplitOptions.None).Length - 1;
                if (newlyComplete > 0) { progressWriter.IncrementAndWrite(newlyComplete); }
            });
            progressWriter?.WriteLast();
            PromptAboutErrorsAndOutput(standardOutputFilePath, standardErrorFilePath);
        }

        private static void PromptToOpenFile(string message, string filePath)
        {
            Console.Write(message);
            Console.ReadKey(true);
            Console.WriteLine();
            using (Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true })) { }
            Console.Write("Press any key to continue.");
            Console.ReadKey(true);
            Console.WriteLine();
        }

        private static void PromptAboutErrorsAndOutput(string outputFilePath, string errorFilePath)
        {
            string output = File.ReadAllText(outputFilePath);
            const string pressAnyKeyToViewOutput = "Press any key to view the output.";
            if (output.Contains("does not contain any script files"))
            {
                PromptToOpenFile("Some folders did not contain any script files. " + pressAnyKeyToViewOutput, outputFilePath);
            }
            if (File.ReadAllText(errorFilePath) != "")
            {
                PromptToOpenFile("Errors occurred while compiling. Press any key to view the errors.", errorFilePath);
            }
            else if (!output.Contains(", 0 failed."))
            {
                PromptToOpenFile("No errors were logged, but the output indicated some failures. " + pressAnyKeyToViewOutput, errorFilePath);
            }
        }

        public static void Run(string sourceDirectoryOrFilePath, string importPath, string outputPath, string outputAndErrorFolder)
        {
            string outputFilePath = outputAndErrorFolder + "Output.txt";
            string errorFilePath = outputAndErrorFolder + "Error.txt";
            File.Delete(outputFilePath);
            File.Delete(errorFilePath);
            Run(sourceDirectoryOrFilePath, importPath, outputPath, outputFilePath, errorFilePath);
        }

        public static string FixReferenceName(string referenceName)
        {
            //Papyrus compiler somehow treats properties with "temp" in them in a special way, so we change them to tmp to accommodate that.
            //WTM:  Note:  In the original PHP version, this was case sensitive.  I'm leaving it as such.
            return referenceName.Replace("temp", "tmp");
        }
    }
}