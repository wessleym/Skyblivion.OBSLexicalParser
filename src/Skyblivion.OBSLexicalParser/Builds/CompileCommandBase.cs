using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.Builds
{
    abstract class CompileCommandBase : ICompileCommand
    {
        public void Compile(string sourcePath, string importPath, string outputPath, string standardOutputFilePath, string standardErrorFilePath)
        {
            PapyrusCompiler.Run(sourcePath, importPath, outputPath, standardOutputFilePath, standardErrorFilePath);
        }
    }
}
