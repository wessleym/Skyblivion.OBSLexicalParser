using Skyblivion.OBSLexicalParser.TES5.Service;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class FindFillerCommand : LPCommand
    {
        public FindFillerCommand()
               : base("skyblivion:parser:findfiller", "Find Filler", "Find OBScript commands that are replaced with fillers")
        { }

        public override void Execute()
        {
            BuildTargetCommand buildTargetCommand = new BuildTargetCommand();
            buildTargetCommand.Execute(false);
            TES5FunctionFactoryUseTracker.WriteTableOfUnknownFunctions();
        }
    }
}
