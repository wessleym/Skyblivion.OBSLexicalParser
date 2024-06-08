using System.IO;

namespace Skyblivion.OBSLexicalParser.Data
{
    static class DataDirectory
    {
        private static readonly string path = "Data" + Path.DirectorySeparatorChar;
        public const string TES4GameFileName = "Oblivion.esm";

        public static readonly string ESMDirectoryPath = path;

        public static readonly string ESMDefaultFilePath = ESMDirectoryPath + TES4GameFileName;

        public static readonly string GraphDirectoryPath = path + "Graph" + Path.DirectorySeparatorChar;

        public static string GetGraphPath(string fileName)
        {
            return GraphDirectoryPath + fileName;
        }

        public static readonly string BuildPath = path + "Build" + Path.DirectorySeparatorChar;

        public static readonly string BuildTargetsPath = path + "BuildTargets" + Path.DirectorySeparatorChar;

        public static readonly string CompilerDirectoryPath = path + "Compiler" + Path.DirectorySeparatorChar;

        public static readonly string ModifiedScriptsDirectoryPathBase = path + "ModifiedScripts" + Path.DirectorySeparatorChar;
        public static readonly string ModifiedScriptsInputsDirectoryPath = ModifiedScriptsDirectoryPathBase + "Inputs" + Path.DirectorySeparatorChar;
        public static readonly string ModifiedScriptsOutputsDirectoryPath = ModifiedScriptsDirectoryPathBase + "Outputs" + Path.DirectorySeparatorChar;
        public static readonly string ModifiedScriptsPreprocessedDirectoryPath = ModifiedScriptsDirectoryPathBase + "Preprocessed" + Path.DirectorySeparatorChar;
        public static readonly string ModifiedScriptsSKSESourceDirectoryPath = ModifiedScriptsDirectoryPathBase + "SKSESource" + Path.DirectorySeparatorChar;
        public static readonly string ModifiedScriptsImportDirectoryPath = ModifiedScriptsDirectoryPathBase + "Import" + Path.DirectorySeparatorChar;
    }
}