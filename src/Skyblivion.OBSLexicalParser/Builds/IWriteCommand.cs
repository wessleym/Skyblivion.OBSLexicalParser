using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IWriteCommand
    {
        void Write(IBuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter);
    }
}