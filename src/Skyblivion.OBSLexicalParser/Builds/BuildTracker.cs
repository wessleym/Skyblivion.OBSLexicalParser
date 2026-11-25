using Skyblivion.OBSLexicalParser.TES5.AST;
using System;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTracker
    {
        //WTM:  Change:  Unused:  private readonly BuildTargetCollection buildTargetCollection;
        /*
             * Map build target => built TES5 scripts
        */
        private readonly Dictionary<string, Dictionary<string, TES5Script>> builtScripts = new Dictionary<string, Dictionary<string, TES5Script>>();
        /*
        * BuildTracker constructor.
        */
        public BuildTracker(IEnumerable<IBuildTarget> buildTargets)
        {
            //this.buildTargetCollection = buildTargetCollection;
            foreach (var buildTarget in buildTargets)
            {
                this.builtScripts.Add(buildTarget.Name, new Dictionary<string, TES5Script>(StringComparer.OrdinalIgnoreCase));
            }
        }

        public void RegisterBuiltScript(IBuildTarget buildTarget, TES5Script script)
        {
            this.builtScripts[buildTarget.Name][script.ScriptHeader.OriginalScriptName] = script;
        }

        public Dictionary<string, TES5Script> GetBuiltScripts(string targetName)
        {
            return builtScripts[targetName];
        }
    }
}