using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IASTCommand
    {
        ITES4CodeFilterable Parse(string sourcePath);
    }
}