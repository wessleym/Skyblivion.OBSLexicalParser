using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class WriteCommand : IWriteCommand
    {
        public void write(BuildTarget target, BuildTracker buildTracker)
        {
            var scripts = buildTracker.getBuiltScripts(target.getTargetName());
            foreach (var script in scripts.Values)
            {
                File.WriteAllLines(script.getOutputPath(), script.getScript().output());
            }
        }
    }
}