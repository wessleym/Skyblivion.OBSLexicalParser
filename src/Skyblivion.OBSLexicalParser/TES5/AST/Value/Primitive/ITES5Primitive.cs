using Skyblivion.OBSLexicalParser.TES5.AST.Code;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Value.Primitive
{
    interface ITES5Primitive : ITES5Value, ITES5CodeChunk//WTM:  Change:  Added ITES5CodeChunk.
    {
        object getValue();
    }
}