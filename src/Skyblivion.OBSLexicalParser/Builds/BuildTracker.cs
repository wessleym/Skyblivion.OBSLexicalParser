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
        public BuildTracker(BuildTargetCollection buildTargetCollection)
        {
            //this.buildTargetCollection = buildTargetCollection;
            foreach (var buildTarget in buildTargetCollection)
            {
                this.builtScripts.Add(buildTarget.GetTargetName(), new Dictionary<string, TES5Target>());
            }
        }

        public void RegisterBuiltScript(BuildTarget buildTarget, TES5Target script)
        {
            this.builtScripts[buildTarget.GetTargetName()][script.Script.ScriptHeader.OriginalScriptName] = script;
        }

        public Dictionary<string, TES5Target> GetBuiltScripts(string targetName)
        {
            return builtScripts[targetName];
        }
    }
}