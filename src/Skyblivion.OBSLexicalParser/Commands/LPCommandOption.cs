namespace Skyblivion.OBSLexicalParser.Commands
{
    public class LPCommandOption : LPCommandArgumentOrOption
    {
        private string shortName;
        public LPCommandOption(string name, string shortName, string description, string defaultValue = null)
            : base(name, description, defaultValue)
        {
            this.shortName = shortName;
        }
    }
}
