using Skyblivion.OBSLexicalParser.Commands;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class WriteCommand : WriteCommandBase, IWriteCommand
    {
        public void Write(BuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            Write(buildTracker.GetBuiltScripts(target.GetTargetName()).Values, progressWriter);
        }
    }
}