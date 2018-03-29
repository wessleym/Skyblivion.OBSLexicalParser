using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class Build
    {
        public static readonly string BasePathFromCurrentDirectory = "Data" + Path.DirectorySeparatorChar;
        public static readonly string DEFAULT_BUILD_PATH = BasePathFromCurrentDirectory + "Build" + Path.DirectorySeparatorChar;
        private string buildPath;
        public Build(string buildPath)
        {
            if (!buildPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                buildPath += Path.DirectorySeparatorChar;
            }
            this.buildPath = buildPath;
        }

        public string getBuildPath()
        {
            return this.buildPath;
        }

        public string getErrorLogPath()
        {
            return this.getBuildPath() + "build_error_log";
        }

        public string getCompileStandardOutputPath()
        {
            return this.getBuildPath() + "compile_stdout_log";
        }

        public string getCompileStandardErrorPath()
        {
            return this.getBuildPath() + "compile_stderr_log";
        }

        public string getWorkspacePath()
        {
            return this.getBuildPath() + "Workspace" + Path.DirectorySeparatorChar;
        }

        public bool canBuild()
        {
            return !Directory.EnumerateFileSystemEntries(this.getWorkspacePath()).Any();
        }
    }
}