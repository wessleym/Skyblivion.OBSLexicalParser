using Skyblivion.OBSLexicalParser.Commands;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class WriteCommand : WriteCommandBase, IWriteCommand
    {
        public void Write(IBuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            Write(buildTracker.GetBuiltScripts(target.Name).Values, progressWriter);
        }
    }
}