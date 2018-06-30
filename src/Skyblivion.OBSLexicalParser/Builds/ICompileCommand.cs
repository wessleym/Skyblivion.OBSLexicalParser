namespace Skyblivion.OBSLexicalParser.Builds
{
    public interface ICompileCommand
    {
        void Compile(string sourcePath, string workspacePath, string outputPath, string standardOutputFilePath, string standardErrorFilePath);
    }
}