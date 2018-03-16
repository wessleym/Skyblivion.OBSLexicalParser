using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.TES5.Factory
{
    interface ITES5CodeChunkFactory
    {
        TES5CodeChunkCollection createCodeChunk(ITES4CodeChunk chunk, TES5CodeScope codeScope,  TES5GlobalScope globalScope,  TES5MultipleScriptsScope multipleScriptsScope);
    }
}