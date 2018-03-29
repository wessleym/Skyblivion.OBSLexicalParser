using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IASTCommand
    {
        void initialize();
        ITES4CodeFilterable getAST(string sourcePath);
    }
}