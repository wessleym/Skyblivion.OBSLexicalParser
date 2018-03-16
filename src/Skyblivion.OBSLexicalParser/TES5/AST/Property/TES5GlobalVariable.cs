namespace Skyblivion.OBSLexicalParser.TES5.AST.Property
{
    class TES5GlobalVariable
    {
        private string name;
        public TES5GlobalVariable(string name)
        {
            this.name = name;
        }

        public string getName()
        {
            return this.name;
        }
    }
}