using Skyblivion.OBSLexicalParser.TES4.Types;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Value.Primitive
{
    interface ITES4Primitive : ITES4Value
    {
        TES4Type getType();
    }
}