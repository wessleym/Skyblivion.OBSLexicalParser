using Skyblivion.OBSLexicalParser.TES4.Types;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive
{
    interface ITES4Primitive : ITES4ValueString
    {
        TES4Type Type { get; }
    }
}