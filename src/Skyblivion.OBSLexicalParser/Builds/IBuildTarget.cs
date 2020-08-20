namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IBuildTarget
    {
        string Name { get; }
        bool CanBuild(bool deleteFiles);
        string GetTranspileToPath(string scriptName);
    }
}
