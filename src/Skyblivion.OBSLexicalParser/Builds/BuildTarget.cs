using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.Utilities;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    class BuildTarget
    {
        public const string BUILD_TARGET_STANDALONE = "Standalone";
        public const string BUILD_TARGET_TIF = "TIF";
        public const string BUILD_TARGET_QF = "QF";
        public const string BUILD_TARGET_PF = "PF";
        public const string DEFAULT_TARGETS = BUILD_TARGET_STANDALONE+ "," + BUILD_TARGET_TIF+ "," +BUILD_TARGET_QF;
        //public static string StandaloneSourcePath = Path.Combine(DataDirectory.GetBuildTargetsPath(), "Standalone", "Source") + Path.DirectorySeparatorChar;
        private readonly string targetName;
        private readonly string filePrefix;
        private readonly Build build;
        private readonly ITranspileCommand transpileCommand;
        private readonly ICompileCommand compileCommand;
        private readonly IASTCommand astCommand;
        private readonly IBuildScopeCommand buildScopeCommand;
        private readonly IWriteCommand writeCommand;
        public BuildTarget(string targetName, string filePrefix, Build build, ITranspileCommand transpileCommand, ICompileCommand compileCommand, IASTCommand astCommand, IBuildScopeCommand buildScopeCommand, IWriteCommand writeCommand)
        {
            this.targetName = targetName;
            this.build = build;
            this.filePrefix = filePrefix;
            this.transpileCommand = transpileCommand;
            this.compileCommand = compileCommand;
            this.astCommand = astCommand;
            this.buildScopeCommand = buildScopeCommand;
            this.writeCommand = writeCommand;
        }

        public TES5Target Transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope compilingScope)
        {
            return this.transpileCommand.Transpile(sourcePath, outputPath, globalScope, compilingScope);
        }

        public void Compile(string sourcePath, string workspacePath, string outputPath, string standardOutputFilePath, string standardErrorFilePath)
        {
            this.compileCommand.Compile(sourcePath, workspacePath, outputPath, standardOutputFilePath, standardErrorFilePath);
        }

        public ITES4CodeFilterable GetAST(string sourcePath)
        {
            return this.astCommand.Parse(sourcePath);
        }

        public TES5GlobalScope BuildScope(string sourcePath, TES5GlobalVariables globalVariables)
        {
            return this.buildScopeCommand.Build(sourcePath, globalVariables);
        }

        public void Write(BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            this.writeCommand.Write(this, buildTracker, progressWriter);
        }

        public string GetTargetName()
        {
            return this.targetName;
        }

        public string GetSourcePath()
        {
            return Path.Combine(this.GetRootBuildTargetPath(), "Source")+Path.DirectorySeparatorChar;
        }

        public string GetDependenciesPath()
        {
            return Path.Combine(this.GetRootBuildTargetPath(), "Dependencies")+ Path.DirectorySeparatorChar;
        }

        public string GetArchivePath()
        {
            return Path.Combine(this.GetRootBuildTargetPath(), "Archive")+ Path.DirectorySeparatorChar;
        }

        public string GetArchivedBuildPath(int buildNumber)
        {
            return Path.Combine(this.GetRootBuildTargetPath(), "Archive", buildNumber.ToString()) + Path.DirectorySeparatorChar;
        }

        public string GetSourceFromPath(string scriptName)
        {
            return this.GetSourcePath()+scriptName+".txt";
        }

        public string GetWorkspaceFromPath(string scriptName)
        {
            return this.build.GetWorkspacePath() + scriptName + ".psc";
        }

        public string GetTranspiledPath()
        {
            return this.build.CombineWithBuildPath(Path.Combine("Transpiled", this.targetName)) + Path.DirectorySeparatorChar;
        }

        public string GetArtifactsPath()
        {
            return this.build.CombineWithBuildPath(Path.Combine("Artifacts", this.targetName)) + Path.DirectorySeparatorChar;
        }

        public string GetWorkspacePath()
        {
            return this.build.GetWorkspacePath();
        }

        public string GetTranspileToPath(string scriptName)
        {
            string transformedName = NameTransformer.Limit(scriptName, this.filePrefix);
            return this.GetTranspiledPath()+this.filePrefix+transformedName+".psc";
        }

        public string GetCompileToPath(string scriptName)
        {
            return this.GetArtifactsPath()+scriptName+".pex";
        }

        private string GetRootBuildTargetPath()
        {
            return Path.Combine(DataDirectory.GetBuildTargetsPath(), this.GetTargetName()) + Path.DirectorySeparatorChar;
        }

        public bool CanBuild(bool deleteFiles)
        {
            return Build.CanBuildIn(GetTranspiledPath(), deleteFiles) && Build.CanBuildIn(GetArtifactsPath(), deleteFiles) && this.build.CanBuild(deleteFiles);
        }

        /*
        * Get the sources file list
        * If intersected source files is not null, they will be intersected with build target source files,
        * otherwise all files will be claimed
        */
        public string[] GetSourceFileList(string[]? intersectedSourceFiles = null)
        {
            /*
             * Only files without extension or .txt are considered sources
             * You can add metadata next to those files, but they cannot have those extensions.
             */
            string[] sourcePaths = Directory.EnumerateFiles(GetSourcePath(), "*.txt").Select(path=>Path.GetFileName(path)).ToArray();
            if (intersectedSourceFiles != null)
            {
                sourcePaths = sourcePaths.Where(p=>intersectedSourceFiles.Contains(p)).ToArray();
            }
            return sourcePaths;
        }
    }
}