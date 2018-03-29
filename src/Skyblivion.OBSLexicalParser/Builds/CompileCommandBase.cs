using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.Builds
{
    abstract class CompileCommandBase : ICompileCommand
    {
        public abstract void initialize();

        public void compile(string sourcePath, string workspacePath, string outputPath, string standardOutputFilePath, string standardErrorFilePath)
        {
            PapyrusCompiler.Run(sourcePath, workspacePath, outputPath, standardOutputFilePath, standardErrorFilePath);
        }
    }
}
