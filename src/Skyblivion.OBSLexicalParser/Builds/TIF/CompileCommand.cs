using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.Builds.TIF
{
    class CompileCommand : ICompileCommand
    {
        public void initialize()
        {
        // TODO: Implement initialize() method.
        }

        public string[] compile(string sourcePath, string workspacePath, string outputPath)
        {
            return ExternalExecution.RunPapyrusCompiler(sourcePath, workspacePath, outputPath);
        }
    }
}