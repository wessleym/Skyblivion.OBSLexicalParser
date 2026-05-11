using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value
{
    interface ITES4ValueString : ITES4ValueConstant, ITES4CodeChunkContainer
    {
        string StringValue { get; }
    }
}
