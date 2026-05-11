using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.FunctionCall
{
    class TES4FunctionCall : ITES4CodeChunk
    {
        public string FunctionName { get; }
        public TES4FunctionCall(string functionName)
        {
            this.FunctionName = functionName;
        }
    }
}