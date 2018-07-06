using Skyblivion.OBSLexicalParser.Commands;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds.PF
{
    class WriteCommand : WriteCommandBase, IWriteCommand
    {
        public void write(BuildTarget target, BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            Write(buildTracker.GetBuiltScripts(target.GetTargetName()).Values, progressWriter);
        }
    }
}