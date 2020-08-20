using Skyblivion.OBSLexicalParser.Commands;

namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IWriteCommand
    {
        void Write(IBuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter);
    }
}