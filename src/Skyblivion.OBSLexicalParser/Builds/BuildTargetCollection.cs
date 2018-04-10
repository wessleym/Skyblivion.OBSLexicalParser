using Skyblivion.ESReader.Extensions.IDictionaryExtensions;
using Skyblivion.ESReader.PHP;
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

        public void add(BuildTarget buildTarget)
        {
            this.buildTargets.Add(buildTarget.getTargetName(), buildTarget);
        }

        public bool canBuild()
        {
            return buildTargets.All(bt => bt.Value.canBuild());
        }

        public BuildTarget getByName(string name)
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

        public BuildTarget getByNameOrNull(string name)
        {
            return this.buildTargets.GetWithFallback(name, () => null);
        }

        /*
        * Get source files, assigned per-build target
        * If intersected source files is not null, they will be intersected with build target source files,
        * otherwise all files will be claimed
        */
        public BuildSourceFilesCollection getSourceFiles(string[] intersectedSourceFiles = null)
        {
            BuildSourceFilesCollection collection = new BuildSourceFilesCollection();
            foreach (var buildTarget in this.buildTargets.Select(kvp=>kvp.Value))
            {
                collection.add(buildTarget, buildTarget.getSourceFileList(intersectedSourceFiles));
            }

            return collection;
        }

        public int getTotalSourceFiles()
        {
            BuildSourceFilesCollection sourceFiles = this.getSourceFiles();
            return sourceFiles.Sum(sf => sf.Value.Length);
        }

        /*
        * Plan the build against N workers
        */
        public Dictionary<int, List<Dictionary<string, List<string>>>> getBuildPlan(int workers)
        {
            BuildSourceFilesCollection sourceFiles = this.getSourceFiles();
            TES5BuildPlanBuilder buildPlanBuilder = new TES5BuildPlanBuilder(this.getDependencyGraph());
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

        public string[] getScriptsToCompile(string scriptName)
        {
            return this.getDependencyGraph().getScriptsToCompile(scriptName);
        }

        private string getUniqueBuildFingerprint()
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

        private TES5ScriptDependencyGraph getDependencyGraph()
        {
            return this.dependencyGraph.Value;
        }

        private string GetFilePath()
        {
            return DataDirectory.GetGraphPath("graph_" + this.getUniqueBuildFingerprint());
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