namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IWriteCommand
    {
        void write(BuildTarget target, BuildTracker buildTracker);
    }
}