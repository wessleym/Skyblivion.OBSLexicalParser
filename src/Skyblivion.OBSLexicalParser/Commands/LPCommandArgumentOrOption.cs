namespace Skyblivion.OBSLexicalParser.Commands
{
    public class LPCommandArgumentOrOption
    {
        public readonly string Name;
        private readonly string description, defaultValue;
        private readonly string userValue = null;//WTM:  Note:  Never gets set.  This isn't developed yet and may never be necessary.
        protected LPCommandArgumentOrOption(string name, string description, string defaultValue = null)
        {
            Name = name;
            this.description = description;
            this.defaultValue = defaultValue;
        }
        public string Value => userValue != null ? userValue : defaultValue;
    }
}
