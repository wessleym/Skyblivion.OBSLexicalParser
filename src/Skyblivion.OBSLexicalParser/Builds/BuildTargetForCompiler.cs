namespace Skyblivion.OBSLexicalParser.Builds
{
    internal class BuildTargetForCompiler : IBuildTargetForCompiler
    {
        public string Name { get; }
        public ICompileCommand CompileCommand { get; }
        private readonly string transpiledPath;
        private readonly string artifactsPath;
        public BuildTargetForCompiler(string transpiledPath, string artifactsPath)
        {
            this.Name = "Ad Hoc";
            CompileCommand = new Standalone.CompileCommand();
            this.transpiledPath = transpiledPath;
            this.artifactsPath = artifactsPath;
        }

        public string GetTranspiledPath()
        {
            return transpiledPath;
        }

        public string GetArtifactsPath()
        {
            return artifactsPath;
        }
    }
}
