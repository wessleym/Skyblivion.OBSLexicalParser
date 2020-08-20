using Skyblivion.OBSLexicalParser.TES4.AST.Code;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTargetSimple : IBuildTarget
    {
        private readonly BuildTarget buildTarget;
        private readonly ICompileCommand compileCommand;
        private readonly IASTCommand astCommand;
        public string Name => buildTarget.Name;
        public BuildTargetSimple(BuildTarget buildTarget, ICompileCommand compileCommand, IASTCommand astCommand)
        {
            this.buildTarget = buildTarget;
            this.compileCommand = compileCommand;
            this.astCommand = astCommand;
        }

        public void Compile(string sourcePath, string workspacePath, string outputPath, string standardOutputFilePath, string standardErrorFilePath)
        {
            this.compileCommand.Compile(sourcePath, workspacePath, outputPath, standardOutputFilePath, standardErrorFilePath);
        }

        public ITES4CodeFilterable GetAST(string sourcePath)
        {
            return this.astCommand.Parse(sourcePath);
        }

        public string GetTranspiledPath()
        {
            return this.buildTarget.GetTranspiledPath();
        }

        public string GetTranspileToPath(string scriptName)
        {
            return this.buildTarget.GetTranspileToPath(scriptName);
        }

        public string GetWorkspacePath()
        {
            return this.buildTarget.GetWorkspacePath();
        }

        public string GetWorkspaceFromPath(string scriptName)
        {
            return this.buildTarget.GetWorkspaceFromPath(scriptName);
        }

        public string GetArtifactsPath()
        {
            return this.buildTarget.GetArtifactsPath();
        }

        public bool CanBuild(bool deleteFiles)
        {
            return this.buildTarget.CanBuild(deleteFiles);
        }
    }
}
