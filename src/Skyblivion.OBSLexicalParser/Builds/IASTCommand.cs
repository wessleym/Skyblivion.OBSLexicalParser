using Skyblivion.OBSLexicalParser.TES4.AST;

namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IASTCommand
    {
        void initialize();
        TES4Script getAST(string sourcePath);
    }
}