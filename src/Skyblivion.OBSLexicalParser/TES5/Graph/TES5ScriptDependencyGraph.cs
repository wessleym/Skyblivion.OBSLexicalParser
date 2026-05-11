using Skyblivion.OBSLexicalParser.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Graph
{
    public class TES5ScriptDependencyGraph
    {
        //WTM:  Change:  Graph and UsageGraph were made public so they could be serialized by DataContractSerializer.
        public Dictionary<string, List<string>> Graph { get; }
        public Dictionary<string, List<string>> UsageGraph { get; }
        public static readonly string ErrorLogPath = DataDirectory.GetGraphPath("graph_error_log.txt");
        public static readonly string DebugLogPath = DataDirectory.GetGraphPath("graph_debug_log.txt");

        public TES5ScriptDependencyGraph(Dictionary<string, List<string>> graph, Dictionary<string, List<string>> usageGraph)
        {
            this.Graph = graph;
            this.UsageGraph = usageGraph;
        }
        private TES5ScriptDependencyGraph()//XmlSerializer requires a parameterless constructor.
            : this(new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase), new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase))
        { }

        public string[] GetScriptsToCompile(string scriptName)
        {
            string scriptNameLower = scriptName
#if !PRESERVECASES
                .ToLower()
#endif
                ;
            List<string> dependenciesFound = new List<string>() { scriptNameLower };
            return (new string[] { scriptNameLower }).Concat(this.GetDependenciesFor(scriptName, dependenciesFound)).ToArray();
        }

        /*
        * Finds dependencies to this script.
        * Will resolve both the scripts this script depends on and the scripts that depend on this script
        */
        private List<string> GetDependenciesFor(string scriptName, List<string> foundDependencies)
        {
            List<string> dependencies = new List<string>();
            List<string>? graphOfScriptName;
            if (this.Graph.TryGetValue(scriptName, out graphOfScriptName))
            {
                foreach (var dependencyScript in graphOfScriptName)
                {
                    if (foundDependencies.Contains(dependencyScript, StringComparer.OrdinalIgnoreCase))
                    {
                        continue; //Do not resolve the cycle
                    }
                    dependencies.Add(dependencyScript);
                    foundDependencies.Add(dependencyScript);
                    dependencies.AddRange(this.GetDependenciesFor(dependencyScript, foundDependencies));
                }
            }

            List<string>? usageGraphOfScriptName;
            if (this.UsageGraph.TryGetValue(scriptName, out usageGraphOfScriptName))
            {
                foreach (var usingScript in usageGraphOfScriptName)
                {
                    if (foundDependencies.Contains(usingScript, StringComparer.OrdinalIgnoreCase))
                    {
                        continue; //Do not resolve the cycle
                    }
                    dependencies.Add(usingScript);
                    foundDependencies.Add(usingScript);
                    dependencies.AddRange(this.GetDependenciesFor(usingScript, foundDependencies));
                }
            }

            return dependencies;
        }
    }
}