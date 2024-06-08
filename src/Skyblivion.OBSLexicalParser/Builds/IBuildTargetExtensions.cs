using Skyblivion.ESReader.Extensions;
using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    static class IBuildTargetExtensions
    {
        public static bool IsQF(this IBuildTarget buildTarget)
        {
            return buildTarget.Name == BuildTargetFactory.QFName;
        }

        public static bool IsTIF(this IBuildTarget buildTarget)
        {
            return buildTarget.Name == BuildTargetFactory.TIFName;
        }

        public static string GetRootBuildTargetPath(this IBuildTarget buildTarget)
        {
            return Path.Combine(DataDirectory.BuildTargetsPath, buildTarget.Name) + Path.DirectorySeparatorChar;
        }

        public static string GetSourcePath(this IBuildTarget buildTarget)
        {
            return Path.Combine(buildTarget.GetRootBuildTargetPath(), "Source") + Path.DirectorySeparatorChar;
        }

        public static string GetSourceFromPath(this IBuildTarget buildTarget, string scriptName)
        {
            return buildTarget.GetSourcePath() + scriptName + ".txt";
        }

        public static string GetArchivePath(this IBuildTarget buildTarget)
        {
            return Path.Combine(buildTarget.GetRootBuildTargetPath(), "Archive") + Path.DirectorySeparatorChar;
        }

        public static string GetArchivedBuildPath(this IBuildTarget buildTarget, int buildNumber)
        {
            return Path.Combine(buildTarget.GetRootBuildTargetPath(), "Archive", buildNumber.ToString()) + Path.DirectorySeparatorChar;
        }

        /*
        * Get the sources file list
        * If intersected source files is not null, they will be intersected with build target source files,
        * otherwise all files will be claimed
        */
        public static string[] GetSourceFileList(this IBuildTarget buildTarget, string[]? intersectedSourceFiles = null)
        {
            /*
             * Only files without extension or .txt are considered sources
             * You can add metadata next to those files, but they cannot have those extensions.
             */
            string[] sourcePaths = Directory.EnumerateFiles(buildTarget.GetSourcePath(), "*.txt").Select(path => Path.GetFileName(path)).ToArray();
            if (intersectedSourceFiles != null)
            {
                sourcePaths = sourcePaths.Where(p => intersectedSourceFiles.Contains(p)).ToArray();
            }
            return sourcePaths;
        }

        /*
        * Get source files, assigned per-build target
        * If intersected source files is not null, they will be intersected with build target source files,
        * otherwise all files will be claimed
        */
        public static BuildSourceFilesCollection<T> GetSourceFiles<T>(this IEnumerable<T> buildTargets, string[]? intersectedSourceFiles = null) where T : IBuildTarget
        {
            BuildSourceFilesCollection<T> collection = new BuildSourceFilesCollection<T>();
            foreach (var buildTarget in buildTargets)
            {
                collection.Add(buildTarget, buildTarget.GetSourceFileList(intersectedSourceFiles));
            }
            return collection;
        }

        public static int GetTotalSourceFiles(this IEnumerable<IBuildTarget> buildTargets)
        {
            BuildSourceFilesCollection<IBuildTarget> sourceFiles = buildTargets.GetSourceFiles();
            return sourceFiles.Sum(sf => sf.Value.Length);
        }

        private static string GetUniqueBuildFingerprint(this IEnumerable<IBuildTarget> buildTargets)
        {
#if PHP_COMPAT
            string md5 = PHPFunction.MD5("randomseed");
            foreach (var key in this.buildTargets.Select(kvp => kvp.Key).OrderBy(k => k))
            {
                md5 = PHPFunction.MD5(md5 + key);
            }
            return md5;
#else
            string fileName = string.Join("", buildTargets.Select(bt => bt.Name));
            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalid.ToString(), "");
            }
            return fileName;
#endif
        }

        private static string GetFilePath(this IEnumerable<IBuildTarget> buildTargets)
        {
            return DataDirectory.GetGraphPath("graph_" + buildTargets.GetUniqueBuildFingerprint() + ".txt");
        }

        public static TES5ScriptDependencyGraph ReadGraph(this IEnumerable<IBuildTarget> buildTargets)
        {
            string graphPath = buildTargets.GetFilePath();
            string serializedGraph = File.ReadAllText(graphPath);
            return PHPFunction.Deserialize<TES5ScriptDependencyGraph>(serializedGraph);
        }

        public static void WriteGraph(this IEnumerable<IBuildTarget> buildTargets, TES5ScriptDependencyGraph graph)
        {
            string graphPath = buildTargets.GetFilePath();
            string serializedGraph = PHPFunction.Serialize(graph);
            string? graphDirectory = Path.GetDirectoryName(graphPath);
            if (graphDirectory == null) { throw new NullableException(nameof(graphDirectory)); }
            Directory.CreateDirectory(graphDirectory);
            File.WriteAllText(graphPath, serializedGraph);
        }

        private static bool CanBuild(this IEnumerable<IBuildTarget> buildTargets, bool deleteFiles)
        {
            return buildTargets.All(bt => bt.CanBuild(deleteFiles));
        }
        private static bool CanBuild(this IEnumerable<IBuildTarget> buildTargets)
        {
            return buildTargets.CanBuild(false);
        }

        public static bool CanBuildAndWarnIfNot(this IEnumerable<IBuildTarget> buildTargets)
        {
            if (buildTargets.CanBuild()) { return true; }
            Console.WriteLine(DataDirectory.BuildPath + " had old files.  Clear them manually, or use " + BuildFileDeleteCommand.FriendlyNameConst + ".");
            return false;
        }

        public static void DeleteBuildFiles(this IEnumerable<IBuildTarget> buildTargets)
        {
            buildTargets.CanBuild(true);
        }
    }
}
