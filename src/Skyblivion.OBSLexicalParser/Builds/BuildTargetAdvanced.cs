using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTargetAdvanced : IBuildTarget
    {
        private readonly BuildTargetSimple buildTargetSimple;
        private readonly ITranspileCommand transpileCommand;
        private readonly IBuildScopeCommand buildScopeCommand;
        private readonly IWriteCommand writeCommand;
        public string Name => this.buildTargetSimple.Name;
        public BuildTargetAdvanced(BuildTargetSimple buildTargetSimple, ITranspileCommand transpileCommand, IBuildScopeCommand buildScopeCommand, IWriteCommand writeCommand)
        {
            this.buildTargetSimple = buildTargetSimple;
            this.transpileCommand = transpileCommand;
            this.buildScopeCommand = buildScopeCommand;
            this.writeCommand = writeCommand;
        }

        public TES5Target Transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope compilingScope)
        {
            return this.transpileCommand.Transpile(sourcePath, outputPath, globalScope, compilingScope);
        }

        public TES5GlobalScope BuildScope(string sourcePath, TES5GlobalVariables globalVariables)
        {
            return this.buildScopeCommand.Build(sourcePath, globalVariables);
        }

        public void Write(BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            this.writeCommand.Write(this, buildTracker, progressWriter);
        }

        public string GetTranspiledPath()
        {
            return this.buildTargetSimple.GetTranspiledPath();
        }

        public string GetTranspileToPath(string scriptName)
        {
            return this.buildTargetSimple.GetTranspileToPath(scriptName);
        }

        public string GetWorkspacePath()
        {
            return this.buildTargetSimple.GetWorkspacePath();
        }

        public string GetWorkspaceFromPath(string scriptName)
        {
            return this.buildTargetSimple.GetWorkspaceFromPath(scriptName);
        }

        public string GetArtifactsPath()
        {
            return this.buildTargetSimple.GetArtifactsPath();
        }

        public bool CanBuild(bool deleteFiles)
        {
            return this.buildTargetSimple.CanBuild(deleteFiles);
        }
    }
}