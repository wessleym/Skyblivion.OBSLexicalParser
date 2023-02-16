namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    class TES4FunctionCall
    {
        public string FunctionName { get; }
        public TES4FunctionCall(string functionName)
        {
            this.FunctionName = functionName;
        }
    }
}