using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.AST.Block
{
    interface ITES5CodeBlock : ITES5Outputtable
    {
        TES5CodeScope CodeScope { get; }

        TES5FunctionScope FunctionScope { get; }

        void AddChunk(ITES5CodeChunk chunk);

        string BlockName { get; }
    }
}
