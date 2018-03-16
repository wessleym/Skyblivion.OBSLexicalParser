using Skyblivion.ESReader.PHP;
using Skyblivion.OBSLexicalParser.TES5.Graph;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTargetCollection : IEnumerable<BuildTarget>
    {
        private TES5ScriptDependencyGraph dependencyGraph;
        private Dictionary<string, BuildTarget> buildTargets = new Dictionary<string, BuildTarget>();
        public void add(BuildTarget buildTarget)
        {
            this.buildTargets[buildTarget.getTargetName()] = buildTarget;
        }

        public bool canBuild()
        {
            return buildTargets.All(bt => bt.Value.canBuild());
        }

        public string getUniqueBuildFingerprint()
        {
            Dictionary<string, BuildTarget> myBuildTargets = this.buildTargets;
            string md5 = PHPFunction.MD5("randomseed");
            foreach (var key in myBuildTargets.Select(kvp=>kvp.Key).Select(key=>key))
            {
                md5 = PHPFunction.MD5(md5 + key);
            }
            return md5;
        }

        public BuildTarget getByName(string name)
        {
            if (!buildTargets.ContainsKey(name))
            {
                return null;
            }
            return this.buildTargets[name];
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

        private TES5ScriptDependencyGraph getDependencyGraph()
        {
            if (this.dependencyGraph == null)
            {
                this.dependencyGraph = PHPFunction.Deserialize<TES5ScriptDependencyGraph>(File.ReadAllText("app/graph_"+this.getUniqueBuildFingerprint()));
            }

            return this.dependencyGraph;
        }
    }
}