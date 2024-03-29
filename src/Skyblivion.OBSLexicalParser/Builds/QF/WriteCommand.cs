using Dissect.Extensions;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory;
using Skyblivion.OBSLexicalParser.Builds.QF.Factory.Map;
using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.TES5.AST;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds.QF
{
    class WriteCommand : WriteCommandBase, IWriteCommand
    {
        private readonly QFFragmentFactory QFFragmentFactory;
        public WriteCommand(QFFragmentFactory QFFragmentFactory)
        {
            this.QFFragmentFactory = QFFragmentFactory;
        }

        public void Write(IBuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            var scripts = buildTracker.GetBuiltScripts(target.Name).Values;
            List<TES5Target> connectedQuestFragments = new List<TES5Target>();
            Dictionary<string, List<QuestStageScript>> jointScripts = new Dictionary<string, List<QuestStageScript>>();
            /*
             * Scan manually for .map files in the QF scripts folder
             * Reason is that in case we've got a quest with no fragments to anything whatsoever, we'll have to go
             * through it too ( just with empty subfragments trees ), to generate the objective handlings
             */
            string sourcePath = target.GetSourcePath();
            foreach (var mapFilePath in
                Directory.EnumerateFiles(sourcePath, "*.map")
                .Concat(Directory.EnumerateFiles(sourcePath, "*.map2")))
            {
                string mapFileName = Path.GetFileNameWithoutExtension(mapFilePath);
                jointScripts.AddNewListIfNotContainsKey(mapFileName);
            }

            /*
             * Group the fragments together
             */
            foreach (var script in scripts)
            {
                string[] parts = script.Script.ScriptHeader.OriginalScriptName.Split('_');
                if (parts.Length < 3)
                {
                    //Not able to categorize, probably wrong name of the fragment.
                    continue;
                }

                string baseName = parts[0] + "_" + parts[1] + "_" + parts[2];
                jointScripts.AddNewListIfNotContainsKeyAndAddValueToList(baseName, new QuestStageScript(script, int.Parse(parts[3], CultureInfo.InvariantCulture), int.Parse(parts[4], CultureInfo.InvariantCulture)));
            }

            const string joiningQFFragments = "Joining QF Fragments...";
            progressWriter.Write(joiningQFFragments);
            foreach (var kvp in jointScripts)
            {
                var resultingFragmentName = kvp.Key;
                var subfragmentsTrees = kvp.Value;
                TES5Target joinedQF = this.QFFragmentFactory.JoinQFFragments(target, resultingFragmentName, subfragmentsTrees);
                connectedQuestFragments.Add(joinedQF);
            }
            progressWriter.ClearByPreviousProgress(joiningQFFragments);
            //WTM:  Note:  Subtract total scripts and add back connected quest fragments.
            int totalAddend = -scripts.Count + connectedQuestFragments.Count;
            progressWriter.ModifyTotalAndWrite(totalAddend);

            Write(connectedQuestFragments, progressWriter);
        }
    }
}