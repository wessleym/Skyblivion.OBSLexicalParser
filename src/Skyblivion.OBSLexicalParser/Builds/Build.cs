using Skyblivion.OBSLexicalParser.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class Build
    {
        public static readonly string DEFAULT_BUILD_PATH = DataDirectory.GetBuildPath();
        private readonly string buildPath;
        public Build(string buildPath)
        {
            this.buildPath = buildPath;
        }

        public string CombineWithBuildPath(string additionalPath)
        {
            return Path.Combine(this.buildPath, additionalPath);
        }

        public string GetErrorLogPath()
        {
            return CombineWithBuildPath("build_error_log.txt");
        }

        public string GetCompileStandardOutputPath()
        {
            return CombineWithBuildPath("compile_stdout_log.txt");
        }

        public string GetCompileStandardErrorPath()
        {
            return CombineWithBuildPath("compile_stderr_log.txt");
        }

        public string GetWorkspacePath()
        {
            return CombineWithBuildPath("Workspace") + Path.DirectorySeparatorChar;
        }

        public string GetWorkspacePath(string scriptName)
        {
            return this.GetWorkspacePath() + scriptName + ".psc";
        }

        public string GetDependenciesPath()
        {
            return CombineWithBuildPath("Dependencies") + Path.DirectorySeparatorChar;
        }

        public static bool CanBuildIn(string directory, bool deleteFiles)
        {
            IEnumerable<string> fileSystemEntries;
            try
            {
                fileSystemEntries = Directory.EnumerateFileSystemEntries(directory);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(directory);
                return true;
            }
            if (!fileSystemEntries.Any()) { return true; }
            if (deleteFiles)
            {
                Console.WriteLine("Deleting Files from " + directory + "...");
                foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
                {
                    File.Delete(file);
                }
                foreach (string dir in Directory.EnumerateDirectories(directory, "*", SearchOption.AllDirectories))
                {
                    Directory.Delete(dir);
                }
                return true;
            }
            return false;
        }

        public bool CanBuild(bool deleteFiles)
        {
            return CanBuildIn(this.GetWorkspacePath(), deleteFiles);
        }

        public void CreateBuildPathDirectory()
        {
            Directory.CreateDirectory(buildPath);
        }
    }
}