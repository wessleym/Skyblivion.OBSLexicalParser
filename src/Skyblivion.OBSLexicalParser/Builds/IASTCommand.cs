namespace Skyblivion.OBSLexicalParser.Builds
{
    interface IASTCommand<T>
    {
        T Parse(string sourcePath);
    }
}