using Skyblivion.OBSLexicalParser.Utilities;

namespace Skyblivion.OBSLexicalParser.Builds.Standalone
{
    class CompileCommand : ICompileCommand
    {
        public void initialize()
        {

        }

        public string[] compile(string sourcePath, string workspacePath, string outputPath)
        {
            return ExternalExecution.RunPapyrusCompiler(sourcePath, workspacePath, outputPath);
        }
    }
}