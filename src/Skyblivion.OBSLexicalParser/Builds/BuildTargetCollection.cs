using Skyblivion.ESReader.Extensions.IDictionaryExtensions;
using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES5.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTargetCollection : IEnumerable<BuildTarget>
    {
        private Lazy<TES5ScriptDependencyGraph> dependencyGraph;
        private Dictionary<string, BuildTarget> buildTargets;
        public BuildTargetCollection()
        {
            buildTargets = new Dictionary<string, BuildTarget>();
            dependencyGraph = new Lazy<TES5ScriptDependencyGraph>(() => ReadGraph());
        }

        public void Add(BuildTarget buildTarget)
        {
            this.buildTargets.Add(buildTarget.GetTargetName(), buildTarget);
        }

        private bool CanBuild(bool deleteFiles)
        {
            return buildTargets.Values.All(bt=>bt.CanBuild(deleteFiles));
        }
        private bool CanBuild()
        {
            return CanBuild(false);
        }

        public bool CanBuildAndWarnIfNot()
        {
            if (CanBuild()) { return true; }
            Console.WriteLine(DataDirectory.GetBuildPath() + " had old files.  Clear them manually, or use " + BuildFileDeleteCommand.FriendlyNameConst + ".");
            return false;
        }

        public void DeleteBuildFiles()
        {
            CanBuild(true);
        }

        public BuildTarget GetByName(string name)
        {
            try
            {
                return this.buildTargets[name];
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException("Unknown build " + name);
            }
        }

        public BuildTarget GetByNameOrNull(string name)
        {
            return this.buildTargets.GetWithFallback(name, () => null);
        }

        /*
        * Get source files, assigned per-build target
        * If intersected source files is not null, they will be intersected with build target source files,
        * otherwise all files will be claimed
        */
        public BuildSourceFilesCollection GetSourceFiles(string[] intersectedSourceFiles = null)
        {
            BuildSourceFilesCollection collection = new BuildSourceFilesCollection();
            foreach (var buildTarget in this.buildTargets.Values)
            {
                collection.add(buildTarget, buildTarget.GetSourceFileList(intersectedSourceFiles));
            }

            return collection;
        }

        public int GetTotalSourceFiles()
        {
            BuildSourceFilesCollection sourceFiles = this.GetSourceFiles();
            return sourceFiles.Sum(sf => sf.Value.Length);
        }

        /*
        * Plan the build against N workers
        */
        public Dictionary<int, List<Dictionary<string, List<string>>>> getBuildPlan(int workers)
        {
            BuildSourceFilesCollection sourceFiles = this.GetSourceFiles();
            TES5BuildPlanBuilder buildPlanBuilder = new TES5BuildPlanBuilder(this.GetDependencyGraph());
            Dictionary<int, List<Dictionary<string, List<string>>>> buildPlan = buildPlanBuilder.createBuildPlan(sourceFiles, workers);
            return buildPlan;
        }

        public IEnumerator<BuildTarget> GetEnumerator()
        {
            return buildTargets.Select(bt=>bt.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string[] GetScriptsToCompile(string scriptName)
        {
            return this.GetDependencyGraph().getScriptsToCompile(scriptName);
        }

        private string GetUniqueBuildFingerprint()
        {
#if PHP_COMPAT
            string md5 = PHPFunction.MD5("randomseed");
            foreach (var key in this.buildTargets.Select(kvp => kvp.Key).OrderBy(k => k))
            {
                md5 = PHPFunction.MD5(md5 + key);
            }
            return md5;
#else
            string fileName = string.Join("", this.buildTargets.Select(kvp => kvp.Key));
            foreach(char invalid in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalid.ToString(), "");
            }
            return fileName;
#endif
        }

        private TES5ScriptDependencyGraph GetDependencyGraph()
        {
            return this.dependencyGraph.Value;
        }

        private string GetFilePath()
        {
            return DataDirectory.GetGraphPath("graph_" + this.GetUniqueBuildFingerprint());
        }

        private TES5ScriptDependencyGraph ReadGraph()
        {
            return PHPFunction.Deserialize<TES5ScriptDependencyGraph>(File.ReadAllText(GetFilePath()));
        }

        public void WriteGraph(TES5ScriptDependencyGraph graph)
        {
            string graphPath = GetFilePath();
            string serializedGraph = PHPFunction.Serialize(graph);
            Directory.CreateDirectory(Path.GetDirectoryName(graphPath));
            File.WriteAllText(graphPath, serializedGraph);
        }
    }
}