namespace Skyblivion.OBSLexicalParser.Builds
{
    public interface ICompileCommand
    {
        void initialize();
        void compile(string sourcePath, string workspacePath, string outputPath, string standardOutputFilePath, string standardErrorFilePath);
    }
}