using Skyblivion.OBSLexicalParser.Data;
using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Graph
{
    public class TES5ScriptDependencyGraph
    {
        //WTM:  Change:  Graph and UsageGraph were made public so they could be serialized by DataContractSerializer.
        public Dictionary<string, List<string>> Graph = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> UsageGraph = new Dictionary<string, List<string>>();
        public static readonly string ErrorLogPath = DataDirectory.GetGraphPath("graph_error_log");
        public static readonly string DebugLogPath = DataDirectory.GetGraphPath("graph_debug_log");
        private TES5ScriptDependencyGraph()//XmlSerializer requires a parameterless constructor.
        { }

        public TES5ScriptDependencyGraph(Dictionary<string, List<string>> graph, Dictionary<string, List<string>> usageGraph)
            : this()
        {
            this.Graph = graph;
            this.UsageGraph = usageGraph;
        }

        public string[] GetScriptsToCompile(string scriptName)
        {
            string scriptNameLower = scriptName.ToLower();
            List<string> dependenciesFound = new List<string>() { scriptNameLower };
            return (new string[] { scriptNameLower }).Concat(this.GetDependenciesFor(scriptName, dependenciesFound)).ToArray();
        }

        /*
        * Finds dependencies to this script.
        * Will resolve both the scripts this script depends on and the scripts that depend on this script
        */
        private List<string> GetDependenciesFor(string scriptName, List<string> foundDependencies)
        {
            string scriptNameLower = scriptName.ToLower();
            List<string> dependencies = new List<string>();
            List<string> graphOfScriptName;
            if (this.Graph.TryGetValue(scriptNameLower, out graphOfScriptName))
            {
                foreach (var dependencyScript in graphOfScriptName)
                {
                    string dependencyScriptLower = dependencyScript.ToLower();
                    if (foundDependencies.Contains(dependencyScriptLower))
                    {
                        continue; //Do not resolve the cycle
                    }
                    dependencies.Add(dependencyScript);
                    foundDependencies.Add(dependencyScriptLower);
                    dependencies.AddRange(this.GetDependenciesFor(dependencyScript, foundDependencies));
                }
            }

            List<string> usageGraphOfScriptName;
            if (this.UsageGraph.TryGetValue(scriptNameLower, out usageGraphOfScriptName))
            {
                foreach (var usingScript in usageGraphOfScriptName)
                {
                    string usingScriptLower = usingScript.ToLower();
                    if (foundDependencies.Contains(usingScriptLower))
                    {
                        continue; //Do not resolve the cycle
                    }
                    dependencies.Add(usingScript);
                    foundDependencies.Add(usingScriptLower);
                    dependencies.AddRange(this.GetDependenciesFor(usingScript, foundDependencies));
                }
            }

            return dependencies;
        }
    }
}