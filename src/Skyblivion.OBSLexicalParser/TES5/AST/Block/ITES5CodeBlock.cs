using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    interface ITES5CodeBlock : ITES5Outputtable
    {
        TES5CodeScope getCodeScope();
        TES5FunctionScope getFunctionScope();
        void addChunk(ITES5CodeChunk chunk);
    }
}
