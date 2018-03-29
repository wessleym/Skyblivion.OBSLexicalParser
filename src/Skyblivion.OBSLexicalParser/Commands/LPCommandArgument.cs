namespace Skyblivion.OBSLexicalParser.Commands
{
    public class LPCommandArgument : LPCommandArgumentOrOption
    {
        public LPCommandArgument(string name, string description, string defaultValue = null)
            : base(name, description, defaultValue)
        { }
    }
}
