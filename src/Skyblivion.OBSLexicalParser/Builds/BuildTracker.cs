using Skyblivion.OBSLexicalParser.TES5.AST;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTracker
    {
        //WTM:  Change:  Unused:  private readonly BuildTargetCollection buildTargetCollection;
        /*
             * Map build target => built TES5 scripts
        */
        private readonly Dictionary<string, Dictionary<string, TES5Target>> builtScripts = new Dictionary<string, Dictionary<string, TES5Target>>();
        /*
        * BuildTracker constructor.
        */
        public BuildTracker(IEnumerable<IBuildTarget> buildTargets)
        {
            //this.buildTargetCollection = buildTargetCollection;
            foreach (var buildTarget in buildTargets)
            {
                this.builtScripts.Add(buildTarget.Name, new Dictionary<string, TES5Target>());
            }
        }

        public void RegisterBuiltScript(IBuildTarget buildTarget, TES5Target script)
        {
            this.builtScripts[buildTarget.Name][script.Script.ScriptHeader.OriginalScriptName] = script;
        }

        public Dictionary<string, TES5Target> GetBuiltScripts(string targetName)
        {
            return builtScripts[targetName];
        }
    }
}