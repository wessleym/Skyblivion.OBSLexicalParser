using Skyblivion.OBSLexicalParser.TES5.AST.Property.Collection;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    /*
     * Class BuildTarget
     * @package Ormin\OBSLexicalParser\Builds
     */
    class BuildTarget
    {
        public const string BUILD_TARGET_STANDALONE = "Standalone";
        public const string BUILD_TARGET_TIF = "TIF";
        public const string BUILD_TARGET_QF = "QF";
        public const string BUILD_TARGET_PF = "PF";
        public const string DEFAULT_TARGETS = BUILD_TARGET_STANDALONE+ "," + BUILD_TARGET_TIF+ "," +BUILD_TARGET_QF;
        private bool transpileInitialized, compileInitialized, ASTInitialized, scopeInitialized;
        private string targetName;
        private string filePrefix;
        private Build build;
        private ITranspileCommand transpileCommand;
        private ICompileCommand compileCommand;
        private IASTCommand ASTCommand;
        private IBuildScopeCommand buildScopeCommand;
        private IWriteCommand writeCommand;
        /*
        * Needed for proper resolution of filename
        */
        private TES5NameTransformer nameTransformer;
        public BuildTarget(string targetName, string filePrefix, Build build, TES5NameTransformer nameTransformer, ITranspileCommand transpileCommand, ICompileCommand compileCommand, IASTCommand ASTCommand, IBuildScopeCommand buildScopeCommand, IWriteCommand writeCommand)
        {
            this.transpileInitialized = false;
            this.compileInitialized = false;
            this.ASTInitialized = false;
            this.scopeInitialized = false;
            this.targetName = targetName;
            this.build = build;
            this.filePrefix = filePrefix;
            this.transpileCommand = transpileCommand;
            this.compileCommand = compileCommand;
            this.nameTransformer = nameTransformer;
            this.ASTCommand = ASTCommand;
            this.buildScopeCommand = buildScopeCommand;
            this.writeCommand = writeCommand;
        }

        public Skyblivion.OBSLexicalParser.TES5.AST.TES5Target transpile(string sourcePath, string outputPath, TES5GlobalScope globalScope, TES5MultipleScriptsScope compilingScope)
        {
            if (!this.transpileInitialized)
            {
                this.transpileCommand.initialize(this.build);
                this.transpileInitialized = true;
            }

            return this.transpileCommand.transpile(sourcePath, outputPath, globalScope, compilingScope);
        }

        public string[] compile(string sourcePath, string workspacePath, string outputPath)
        {
            if (!this.compileInitialized)
            {
                this.compileCommand.initialize();
                this.compileInitialized = true;
            }

            return this.compileCommand.compile(sourcePath, workspacePath, outputPath);
        }

        public TES4.AST.TES4Script getAST(string sourcePath)
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

        public void write(BuildTracker buildTracker)
        {
            this.writeCommand.write(this, buildTracker);
        }

        public string getTargetName()
        {
            return this.targetName;
        }

        public string getSourcePath()
        {
            return this.getRootBuildTargetPath()+"/Source/";
        }

        public string getDependenciesPath()
        {
            return this.getRootBuildTargetPath()+"/Dependencies/";
        }

        public string getArchivePath()
        {
            return this.getRootBuildTargetPath() + "/Archive/";
        }

        public string getArchivedBuildPath(int buildNumber)
        {
            return this.getRootBuildTargetPath() + "/Archive/" + buildNumber.ToString() + "/";
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
            return this.build.getBuildPath() + "/Transpiled/" + this.targetName + "/";
        }

        public string getArtifactsPath()
        {
            return this.build.getBuildPath() + "/Artifacts/" + this.targetName + "/";
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
            return "./BuildTargets/"+this.getTargetName();
        }

        public bool canBuild()
        {
            return (!(Directory.EnumerateFileSystemEntries(getTranspiledPath()).Any() || Directory.EnumerateFileSystemEntries(getArtifactsPath()).Any()) && this.build.canBuild());
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
            string[] sourcePaths = Directory.EnumerateFiles(getSourcePath(), "*.txt").ToArray();
            if (intersectedSourceFiles != null)
            {
                sourcePaths = sourcePaths.Where(p=>intersectedSourceFiles.Contains(p)).ToArray();
            }
            return sourcePaths;
        }
    }
}