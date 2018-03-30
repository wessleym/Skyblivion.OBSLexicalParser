using Skyblivion.OBSLexicalParser.Commands;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.TES4.AST.Code;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
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
        public static string StandaloneSourcePath = Path.Combine(DataDirectory.GetBuildTargetsPath(), "Standalone", "Source") + Path.DirectorySeparatorChar;
        private bool transpileInitialized, compileInitialized, ASTInitialized, scopeInitialized;
        private string targetName;
        private string filePrefix;
        private Build build;
        private MetadataLogService metadataLogService;
        private ITranspileCommand transpileCommand;
        private ICompileCommand compileCommand;
        private IASTCommand ASTCommand;
        private IBuildScopeCommand buildScopeCommand;
        private IWriteCommand writeCommand;
        /*
        * Needed for proper resolution of filename
        */
        private TES5NameTransformer nameTransformer;
        public BuildTarget(string targetName, string filePrefix, Build build, MetadataLogService metadataLogService, TES5NameTransformer nameTransformer, ITranspileCommand transpileCommand, ICompileCommand compileCommand, IASTCommand ASTCommand, IBuildScopeCommand buildScopeCommand, IWriteCommand writeCommand)
        {
            this.transpileInitialized = false;
            this.compileInitialized = false;
            this.ASTInitialized = false;
            this.scopeInitialized = false;
            this.targetName = targetName;
            this.build = build;
            this.metadataLogService = metadataLogService;
            this.filePrefix = filePrefix;
            this.transpileCommand = transpileCommand;
            this.compileCommand = compileCommand;
            this.nameTransformer = nameTransformer;
            this.ASTCommand = ASTCommand;
            this.buildScopeCommand = buildScopeCommand;
            this.writeCommand = writeCommand;
        }

        public TES5Target transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope compilingScope)
        {
            if (!this.transpileInitialized)
            {
                this.transpileCommand.initialize(this.build, metadataLogService);
                this.transpileInitialized = true;
            }

            return this.transpileCommand.transpile(sourcePath, outputPath, globalScope, compilingScope);
        }

        public void compile(string sourcePath, string workspacePath, string outputPath, string standardOutputFilePath, string standardErrorFilePath)
        {
            if (!this.compileInitialized)
            {
                this.compileCommand.initialize();
                this.compileInitialized = true;
            }

            this.compileCommand.compile(sourcePath, workspacePath, outputPath, standardOutputFilePath, standardErrorFilePath);
        }

        public ITES4CodeFilterable getAST(string sourcePath)
        {
            if (!this.ASTInitialized)
            {
                this.ASTCommand.initialize();
                this.ASTInitialized = true;
            }

            return this.ASTCommand.getAST(sourcePath);
        }

        public TES5GlobalScope buildScope(string sourcePath, TES5GlobalVariables globalVariables)
        {
            if (!this.scopeInitialized)
            {
                this.buildScopeCommand.initialize();
                this.scopeInitialized = true;
            }

            return this.buildScopeCommand.buildScope(sourcePath, globalVariables);
        }

        public void write(BuildTracker buildTracker, ProgressWriter progressWriter)
        {
            this.writeCommand.write(this, buildTracker, progressWriter);
        }

        public string getTargetName()
        {
            return this.targetName;
        }

        public string getSourcePath()
        {
            return Path.Combine(this.getRootBuildTargetPath(), "Source")+Path.DirectorySeparatorChar;
        }

        public string getDependenciesPath()
        {
            return Path.Combine(this.getRootBuildTargetPath(), "Dependencies")+ Path.DirectorySeparatorChar;
        }

        public string getArchivePath()
        {
            return Path.Combine(this.getRootBuildTargetPath(), "Archive")+ Path.DirectorySeparatorChar;
        }

        public string getArchivedBuildPath(int buildNumber)
        {
            return Path.Combine(this.getRootBuildTargetPath(), "Archive", buildNumber.ToString()) + Path.DirectorySeparatorChar;
        }

        public string getSourceFromPath(string scriptName)
        {
            return this.getSourcePath()+scriptName+".txt";
        }

        public string getWorkspaceFromPath(string scriptName)
        {
            return this.build.getWorkspacePath() + scriptName + ".psc";
        }

        public string getTranspiledPath()
        {
            return this.build.GetBuildPath(Path.Combine("Transpiled", this.targetName)) + Path.DirectorySeparatorChar;
        }

        public string getArtifactsPath()
        {
            return this.build.GetBuildPath(Path.Combine("Artifacts", this.targetName)) + Path.DirectorySeparatorChar;
        }

        public string getWorkspacePath()
        {
            return this.build.getWorkspacePath();
        }

        public string getTranspileToPath(string scriptName)
        {
            string transformedName = TES5NameTransformer.transform(scriptName, this.filePrefix);
            return this.getTranspiledPath()+this.filePrefix+transformedName+".psc";
        }

        public string getCompileToPath(string scriptName)
        {
            return this.getArtifactsPath()+scriptName+".pex";
        }

        private string getRootBuildTargetPath()
        {
            return Path.Combine(DataDirectory.GetBuildTargetsPath(), this.getTargetName()) + Path.DirectorySeparatorChar;
        }

        public bool canBuild()
        {
            return !(Directory.EnumerateFileSystemEntries(getTranspiledPath()).Any() || Directory.EnumerateFileSystemEntries(getArtifactsPath()).Any()) && this.build.canBuild();
        }

        /*
        * Get the sources file list
        * If intersected source files is not null, they will be intersected with build target source files,
        * otherwise all files will be claimed
        */
        public string[] getSourceFileList(string[] intersectedSourceFiles = null)
        {
            /*
             * Only files without extension or .txt are considered sources
             * You can add metadata next to those files, but they cannot have those extensions.
             */
            string[] sourcePaths = Directory.EnumerateFiles(getSourcePath(), "*.txt").Select(path=>Path.GetFileName(path)).ToArray();
            if (intersectedSourceFiles != null)
            {
                sourcePaths = sourcePaths.Where(p=>intersectedSourceFiles.Contains(p)).ToArray();
            }
            return sourcePaths;
        }
    }
}