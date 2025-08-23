using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using System.Collections.Generic;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTargetSimple : IBuildTarget, IBuildTargetForCompiler
    {
        private readonly BuildTarget buildTarget;
        public ICompileCommand CompileCommand { get; }
        private readonly IASTCommand<IEnumerable<ITES4CodeChunk>> astCommand;
        public string Name => buildTarget.Name;
        public BuildTargetSimple(BuildTarget buildTarget, ICompileCommand compileCommand, IASTCommand<IEnumerable<ITES4CodeChunk>> astCommand)
        {
            this.buildTarget = buildTarget;
            this.CompileCommand = compileCommand;
            this.astCommand = astCommand;
        }

        public IEnumerable<ITES4CodeChunk> GetAST(string sourcePath)
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
