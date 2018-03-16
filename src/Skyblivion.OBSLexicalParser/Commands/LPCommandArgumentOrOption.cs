namespace Skyblivion.OBSLexicalParser.Commands
{
    class LPCommandArgumentOrOption
    {
        public string Name;
        private string description, defaultValue, userValue = null;
        public LPCommandArgumentOrOption(string name, string description, string defaultValue = null)
        {
            Name = name;
            this.description = description;
            this.defaultValue = defaultValue;
        }
        public string Value => userValue != null ? userValue : defaultValue;
    }
}
