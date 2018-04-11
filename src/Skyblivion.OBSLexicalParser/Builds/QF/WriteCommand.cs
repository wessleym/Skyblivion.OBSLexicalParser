using Dissect.Extensions.IDictionaryExtensions;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.TES5.AST;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.QF
{
    class WriteCommand : WriteCommandBase, IWriteCommand
    {
        private QFFragmentFactory QFFragmentFactory;
        public WriteCommand(QFFragmentFactory QFFragmentFactory)
        {
            this.QFFragmentFactory = QFFragmentFactory;
        }

        public void write(BuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            var scripts = buildTracker.getBuiltScripts(target.getTargetName());
            List<TES5Target> connectedQuestFragments = new List<TES5Target>();
            Dictionary<string, List<QuestStageScript>> jointScripts = new Dictionary<string, List<QuestStageScript>>();
            /*
             * Scan manually for .map files in the QF scripts folder
             * Reason is that in case we"ve got a quest with no fragments to anything whatsoever, we"ll have to go
             * through it too ( just with empty subfragments trees ), to generate the objective handlings
             */
            string sourcePath = target.getSourcePath();
            foreach (var mapFilePath in Directory.EnumerateFiles(sourcePath, "*.map"))
            {
                string mapFileName = Path.GetFileNameWithoutExtension(mapFilePath);
                jointScripts.AddNewListIfNotContainsKey(mapFileName);
            }

            /*
             * Group the fragments together
             */
            foreach (var script in scripts.Values)
            {
                string[] parts = script.Script.ScriptHeader.OriginalScriptName.Split('_');
                if (parts.Length < 3)
                {
                    //Not able to categorize, probably wrong name of the fragment.
                    continue;
                }

                string baseName = parts[0]+"_"+parts[1]+"_"+parts[2];
                jointScripts.AddNewListIfNotContainsKeyAndAddValueToList(baseName, new QuestStageScript(script, int.Parse(parts[3]), int.Parse(parts[4])));
            }

            foreach (var kvp in jointScripts)
            {
                var resultingFragmentName = kvp.Key;
                var subfragmentsTrees = kvp.Value;
                connectedQuestFragments.Add(this.QFFragmentFactory.joinQFFragments(target, resultingFragmentName, subfragmentsTrees));
            }

            Write(connectedQuestFragments, progressWriter);
        }
    }
}