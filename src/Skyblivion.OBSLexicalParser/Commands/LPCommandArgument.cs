namespace Skyblivion.OBSLexicalParser.Commands
{
    class LPCommandArgument : LPCommandArgumentOrOption
    {
        public LPCommandArgument(string name, string description, string defaultValue = null)
            : base(name, description, defaultValue)
        { }
    }
}
