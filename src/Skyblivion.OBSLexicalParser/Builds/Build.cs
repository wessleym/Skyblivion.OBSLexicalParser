using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class Build
    {
        public const string DEFAULT_BUILD_PATH = "./Build/";
        private string buildPath;
        public Build(string buildPath)
        {
            this.buildPath = buildPath;
        }

        public string getBuildPath()
        {
            return this.buildPath;
        }

        public string getErrorLogPath()
        {
            return this.getBuildPath()+"error_log";
        }

        public string getCompileLogPath()
        {
            return this.getBuildPath()+"compile_log";
        }

        public string getWorkspacePath()
        {
            return this.getBuildPath()+"Workspace/";
        }

        public bool canBuild()
        {
            return !Directory.EnumerateFileSystemEntries(this.getWorkspacePath()).Any();
        }
    }
}