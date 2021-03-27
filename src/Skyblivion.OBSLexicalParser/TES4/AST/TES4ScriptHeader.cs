namespace Skyblivion.OBSLexicalParser.TES4.AST
{
    class TES4ScriptHeader
    {
        public string ScriptName { get; }
        public TES4ScriptHeader(string scriptName)
        {
            this.ScriptName = scriptName;
        }
    }
}