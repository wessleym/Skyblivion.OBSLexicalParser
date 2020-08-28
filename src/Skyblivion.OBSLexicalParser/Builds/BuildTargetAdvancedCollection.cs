using Skyblivion.ESReader.Extensions;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTargetAdvancedCollection : IEnumerable<BuildTargetAdvanced>, IDisposable
    {
        public readonly BuildLogServiceCollection buildLogServices;
        public readonly ESMAnalyzer ESMAnalyzer;
        private readonly Lazy<TES5ScriptDependencyGraph> dependencyGraph;
        private readonly Dictionary<string, BuildTargetAdvanced> buildTargets;
        public BuildTargetAdvancedCollection(BuildLogServiceCollection buildLogServices, ESMAnalyzer esmAnalyzer)
        {
            this.buildLogServices = buildLogServices;
            this.ESMAnalyzer = esmAnalyzer;
            buildTargets = new Dictionary<string, BuildTargetAdvanced>();
            dependencyGraph = new Lazy<TES5ScriptDependencyGraph>(() => this.ReadGraph());
        }

        public void Add(BuildTargetAdvanced buildTarget)
        {
            this.buildTargets.Add(buildTarget.Name, buildTarget);
        }

        public BuildTargetAdvanced? GetByNameOrNull(string name)
        {
            return this.buildTargets.GetWithFallbackNullable(name, () => null);
        }

        /*
        * Plan the build against N workers
        */
        public Dictionary<int, List<Dictionary<BuildTargetAdvanced, List<string>>>> GetBuildPlan(int workers)
        {
            BuildSourceFilesCollection<BuildTargetAdvanced> sourceFiles = this.GetSourceFiles();
            TES5BuildPlanBuilder buildPlanBuilder = new TES5BuildPlanBuilder(this.GetDependencyGraph());
            Dictionary<int, List<Dictionary<BuildTargetAdvanced, List<string>>>> buildPlan = buildPlanBuilder.CreateBuildPlan(sourceFiles, workers);
            return buildPlan;
        }

        public string[] GetScriptsToCompile(string scriptName)
        {
            return this.GetDependencyGraph().GetScriptsToCompile(scriptName);
        }

        private TES5ScriptDependencyGraph GetDependencyGraph()
        {
            return this.dependencyGraph.Value;
        }

        public IEnumerator<BuildTargetAdvanced> GetEnumerator()
        {
            return buildTargets.Select(bt => bt.Value).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => buildTargets.Count;

        public void Dispose()
        {
            buildLogServices.Dispose();
            ESMAnalyzer.Dispose();
        }
    }
}