namespace Skyblivion.OBSLexicalParser.TES5.Context
{
    /*
     * Class TES5LocalVariableParameterMeaning
     * @package Ormin\OBSLexicalParser\TES5\Context
     * @method static TES5LocalVariableParameterMeaning ACTIVATOR()
     * @method static TES5LocalVariableParameterMeaning CONTAINER()
     */
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