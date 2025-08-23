namespace Skyblivion.OBSLexicalParser.Data
{
    static class DataDirectory
    {
        public static readonly string Path = "Data" + System.IO.Path.DirectorySeparatorChar;
        public const string TES4GameFileName = "Oblivion.esm";

        public static readonly string ESMDirectoryPath = Path;

        public static readonly string ESMDefaultFilePath = ESMDirectoryPath + TES4GameFileName;

        public static readonly string GraphDirectoryPath = Path + "Graph" + System.IO.Path.DirectorySeparatorChar;

        public static string GetGraphPath(string fileName)
        {
            return GraphDirectoryPath + fileName;
        }

        public static readonly string BuildPath = Path + "Build" + System.IO.Path.DirectorySeparatorChar;

        public static readonly string BuildSourceScriptsFolderDirectoryPathBase = Path + "BuildSourceScriptsFolder" + System.IO.Path.DirectorySeparatorChar;

        public static readonly string BuildTargetsPath = Path + "BuildTargets" + System.IO.Path.DirectorySeparatorChar;

        public static readonly string CompilerDirectoryPath = Path + "Compiler" + System.IO.Path.DirectorySeparatorChar;

        public static readonly string ComparisonDirectoryPath = Path + "Comparison" + System.IO.Path.DirectorySeparatorChar;

        public static readonly string ModifiedScriptsDirectoryPathBase = Path + "ModifiedScripts" + System.IO.Path.DirectorySeparatorChar;
    }
}