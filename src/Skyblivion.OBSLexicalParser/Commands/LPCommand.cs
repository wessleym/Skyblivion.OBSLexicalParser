namespace Skyblivion.OBSLexicalParser.Commands
{
    public abstract class LPCommand
    {
        public string CommandName { get; private set; }
        public string FriendlyName { get; private set; }
        public string Description { get; private set; }
        protected LPCommandInput Input = new LPCommandInput();
        public abstract void execute();
        protected LPCommand(string commandName, string friendlyName, string description)
        {
            CommandName = commandName;
            FriendlyName = friendlyName;
            Description = description;
        }
    }
}
