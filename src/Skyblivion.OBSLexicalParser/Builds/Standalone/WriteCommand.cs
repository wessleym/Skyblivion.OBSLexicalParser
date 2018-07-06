using Skyblivion.OBSLexicalParser.Commands;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class WriteCommand : WriteCommandBase, IWriteCommand
    {
        public void write(BuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            Write(buildTracker.GetBuiltScripts(target.GetTargetName()).Values, progressWriter);
        }
    }
}