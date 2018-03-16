namespace Skyblivion.OBSLexicalParser.TES4.AST
{
    class TES4ScriptHeader
    {
        private string scriptName;
        public TES4ScriptHeader(string scriptName)
        {
            this.scriptName = scriptName;
        }

        public string getScriptName()
        {
            return this.scriptName;
        }
    }
}