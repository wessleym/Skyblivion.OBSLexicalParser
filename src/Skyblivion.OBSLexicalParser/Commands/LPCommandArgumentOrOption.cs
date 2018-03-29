namespace Skyblivion.OBSLexicalParser.Commands
{
    public class LPCommandArgumentOrOption
    {
        public string Name;
        private string description, defaultValue, userValue = null;
        protected LPCommandArgumentOrOption(string name, string description, string defaultValue = null)
        {
            Name = name;
            this.description = description;
            this.defaultValue = defaultValue;
        }
        public string Value => userValue != null ? userValue : defaultValue;
    }
}
