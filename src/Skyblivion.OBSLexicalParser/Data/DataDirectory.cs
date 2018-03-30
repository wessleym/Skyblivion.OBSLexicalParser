﻿using System.IO;

namespace Skyblivion.OBSLexicalParser.Data
{
    static class DataDirectory
    {
        private static readonly string path = "Data" + Path.DirectorySeparatorChar;
        public const string TES4GameFileName = "Oblivion.esm";

        public static string GetESMDirectoryPath()
        {
            return path;
        }

        public static string GetGraphPath(string fileName)
        {
            return path + "Graph" + Path.DirectorySeparatorChar + fileName;
        }

        public static string GetBuildPath()
        {
            return path + "Build" + Path.DirectorySeparatorChar;
        }

        public static string GetBuildTargetsPath()
        {
            return path + "BuildTargets" + Path.DirectorySeparatorChar;
        }

        public static string GetCompilerDirectoryPath()
        {
            return path + "Compiler" + Path.DirectorySeparatorChar;
        }
    }
}