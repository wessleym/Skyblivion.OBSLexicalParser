using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.Builds.PF
{
    class WriteCommand : WriteCommandBase, IWriteCommand
    {
        public void Write(IBuildTarget buildTarget, BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            Write(buildTracker.GetBuiltScripts(buildTarget.Name).Values, progressWriter, buildTarget);
        }
    }
}