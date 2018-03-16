using System.Collections.Generic;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES5.Graph
{
    class TES5ScriptDependencyGraph
    {
        private Dictionary<string, List<string>> graph=new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> usageGraph = new Dictionary<string, List<string>>();
        public TES5ScriptDependencyGraph(Dictionary<string, List<string>> graph, Dictionary<string, List<string>> usageGraph)
        {
            this.graph = graph;
            this.usageGraph = usageGraph;
        }

        public string[] getScriptsToCompile(string scriptName)
        {
            string scriptNameLower = scriptName.ToLower();
            List<string> dependenciesFound = new List<string>() { scriptNameLower };
            return (new string[] { scriptNameLower }).Concat(this.getDependenciesFor(scriptName, dependenciesFound)).ToArray();
        }

        /*
        * Finds dependencies to this script.
         * Will resolve both the scripts this script depends on and the scripts that depend on this script
         * 
         *   cies
        */
        private List<string> getDependenciesFor(string scriptName, List<string> foundDependencies)
        {
            foundDependencies = new List<string>();
            string scriptNameLower = scriptName.ToLower();
            List<string> dependencies = new List<string>();
            if (this.graph.ContainsKey(scriptNameLower))
            {
                foreach (var dependencyScript in this.graph[scriptNameLower])
                {
                    string dependencyScriptLower = dependencyScript.ToLower();
                    if (foundDependencies.Contains(dependencyScriptLower))
                    {
                        continue; //Do not resolve the cycle
                    }

                    dependencies.Add(dependencyScript);
                    foundDependencies.Add(dependencyScriptLower);
                    dependencies.AddRange(this.getDependenciesFor(dependencyScript, foundDependencies));
                }
            }

            if (this.usageGraph.ContainsKey(scriptNameLower))
            {
                foreach (var usingScript in this.usageGraph[scriptNameLower])
                {
                    string usingScriptLower = usingScript.ToLower();
                    if (foundDependencies.Contains(usingScriptLower))
                    {
                        continue; //Do not resolve the cycle
                    }

                    dependencies.Add(usingScript);
                    foundDependencies.Add(usingScriptLower);
                    dependencies.AddRange(this.getDependenciesFor(usingScript, foundDependencies));
                }
            }

            return dependencies;
        }
    }
}