namespace Skyblivion.OBSLexicalParser.TES5.Context
{
    class TES5LocalVariableParameterMeaning
    {
        public string Name { get; private set; }
        private TES5LocalVariableParameterMeaning(string name)
        {
            Name = name;
        }
        
        public static readonly TES5LocalVariableParameterMeaning
            ACTIVATOR = new TES5LocalVariableParameterMeaning("Activator"),
            CONTAINER = new TES5LocalVariableParameterMeaning("Container");
    }
}