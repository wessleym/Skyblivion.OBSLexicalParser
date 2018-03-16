namespace Skyblivion.OBSLexicalParser.Builds
{
    interface ICompileCommand
    {
        void initialize();
        string[] compile(string sourcePath, string workspacePath, string outputPath);
    }
}