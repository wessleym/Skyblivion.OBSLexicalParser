using Dissect.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.TES5.AST;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTracker
    {
        private BuildTargetCollection buildTargetCollection;
        /*
             * Map build target => built TES5 scripts
        */
        private Dictionary<string, Dictionary<string, TES5Target>> builtScripts = new Dictionary<string, Dictionary<string, TES5Target>>();
        /*
        * BuildTracker constructor.
        */
        public BuildTracker(BuildTargetCollection buildTargetCollection)
        {
            this.buildTargetCollection = buildTargetCollection;
            foreach (var buildTarget in buildTargetCollection)
            {
                this.builtScripts.Add(buildTarget.getTargetName(), new Dictionary<string, TES5Target>());
            }
        }

        public void registerBuiltScript(BuildTarget buildTarget, TES5Target script)
        {
            this.builtScripts[buildTarget.getTargetName()][script.Script.ScriptHeader.OriginalScriptName] = script;
        }

        public Dictionary<string, TES5Target> getBuiltScripts(string targetName)
        {
            return builtScripts.GetWithFallback(targetName, () => new Dictionary<string, TES5Target>());
        }
    }
}