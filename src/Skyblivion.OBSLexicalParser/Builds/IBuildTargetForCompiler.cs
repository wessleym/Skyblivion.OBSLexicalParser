namespace Skyblivion.OBSLexicalParser.Builds
{
    internal interface IBuildTargetForCompiler : ITranspiledPathSource
    {
        string Name { get; }
        string GetArtifactsPath();
        ICompileCommand CompileCommand { get; }
    }
}
