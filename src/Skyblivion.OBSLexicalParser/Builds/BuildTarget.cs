using Skyblivion.OBSLexicalParser.Utilities;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTarget : IBuildTarget
    {
        public string Name { get; private set; }
        private readonly string filePrefix;
        private readonly Build build;
        public BuildTarget(string name, string filePrefix, Build build)
        {
            Name = name;
            this.filePrefix = filePrefix;
            this.build = build;
        }

        public string GetTranspiledPath()
        {
            return this.build.CombineWithBuildPath(Path.Combine("Transpiled", Name)) + Path.DirectorySeparatorChar;
        }

        public string GetTranspileToPath(string scriptName)
        {
            string transformedName = NameTransformer.Limit(scriptName, this.filePrefix);
            return this.GetTranspiledPath() + this.filePrefix + transformedName + ".psc";
        }

        public string GetWorkspacePath()
        {
            return this.build.GetWorkspacePath();
        }

        public string GetWorkspaceFromPath(string scriptName)
        {
            return this.build.GetWorkspacePath() + scriptName + ".psc";
        }

        public string GetArtifactsPath()
        {
            return this.build.CombineWithBuildPath(Path.Combine("Artifacts", Name)) + Path.DirectorySeparatorChar;
        }

        public string GetCompileToPath(string scriptName)
        {
            return this.GetArtifactsPath() + scriptName + ".pex";
        }

        public bool CanBuild(bool deleteFiles)
        {
            return Build.CanBuildIn(GetTranspiledPath(), deleteFiles) && Build.CanBuildIn(GetArtifactsPath(), deleteFiles) && this.build.CanBuild(deleteFiles);
        }
    }
}
