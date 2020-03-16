using Skyblivion.OBSLexicalParser.Commands;

namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IWriteCommand
    {
        void Write(BuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter);
    }
}