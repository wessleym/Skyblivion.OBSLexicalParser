using Skyblivion.OBSLexicalParser.Data;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class Build
    {
        public static readonly string DEFAULT_BUILD_PATH = DataDirectory.GetBuildPath();
        private string buildPath;
        public Build(string buildPath)
        {
            this.buildPath = buildPath;
        }

        public string GetBuildPath(string additionalPath)
        {
            return Path.Combine(this.buildPath, additionalPath);
        }

        public string getErrorLogPath()
        {
            return GetBuildPath("build_error_log");
        }

        public string getCompileStandardOutputPath()
        {
            return GetBuildPath("compile_stdout_log");
        }

        public string getCompileStandardErrorPath()
        {
            return GetBuildPath("compile_stderr_log");
        }

        public string getWorkspacePath()
        {
            return GetBuildPath("Workspace") + Path.DirectorySeparatorChar;
        }

        public bool canBuild()
        {
            return !Directory.EnumerateFileSystemEntries(this.getWorkspacePath()).Any();
        }
    }
}