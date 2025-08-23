using Skyblivion.OBSLexicalParser.Builds.TIF;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Builds
{
    internal class BuildTargetExecutor
    {
        public const int DefaultThreads = 1;
        public static bool AllowNewOutput { get; set; } = true;
        private readonly Build build;
        public IReadOnlyList<BuildTarget> BuildTargets { get; }
        private readonly IReadOnlyList<BuildTargetSimple> buildTargetsSimple;
        private BuildTargetExecutor(Build build, IReadOnlyList<BuildTarget> buildTargets, IReadOnlyList<BuildTargetSimple> buildTargetsSimple)
        {
            this.build = build;
            BuildTargets = buildTargets;
            this.buildTargetsSimple = buildTargetsSimple;
        }

        public static BuildTargetExecutor? TryConstruct(string? targets = null, string? buildPath = null)
        {
            if (targets == null) { targets = BuildTargetFactory.DefaultNames; }
            if (buildPath == null) { buildPath = Build.DEFAULT_BUILD_PATH; }
            Build build = new Build(buildPath);
            BuildTarget[] buildTargets = BuildTargetFactory.ParseCollection(targets, build);
            if (!buildTargets.CanBuildAndWarnIfNot()) { return null; }
            BuildTargetSimple[] buildTargetsSimple = BuildTargetFactory.GetCollection(buildTargets);
            return new BuildTargetExecutor(build, buildTargets, buildTargetsSimple);
        }

        private static void Transpile(Build build, BuildTracker buildTracker, BuildTargetAdvancedCollection buildTargets, int threadsNumber)
        {
            var buildPlan = buildTargets.GetBuildPlan(threadsNumber);
            TES5ScriptHeaderFactory scriptHeaderFactory = buildTargets.ScriptHeaderFactory;
            ESMAnalyzer esmAnalyzer = buildTargets.ESMAnalyzer;
            int totalScripts = buildPlan.Values.Sum(v => v.Sum(chunk => chunk.Values.Sum(c => c.Count)));
            ProgressWriter progressWriter = new ProgressWriter("Transpiling Scripts", totalScripts);
            using (StreamWriter errorLog = new StreamWriter(build.GetErrorLogPath(), false))
            {
                foreach (var threadBuildPlan in buildPlan)
                {
                    TranspileChunkJob task = new TranspileChunkJob(buildTracker, threadBuildPlan.Value, scriptHeaderFactory, esmAnalyzer);
                    task.RunTask(errorLog, progressWriter);
                }
            }
            const string generating = "Generating INFO AddTopic Scripts...";
            progressWriter.Write(generating);
            AddTopicBuilderCommand.GenerateINFOAddTopicScripts(scriptHeaderFactory, esmAnalyzer, buildTracker, buildTargets.First(t => t.IsTIF()));
            progressWriter.ClearByPreviousProgress(generating);
            progressWriter.WriteLast();
        }

        private static void WriteTranspiled(BuildTargetAdvancedCollection buildTargets, BuildTracker buildTracker)
        {
            ProgressWriter progressWriter = new ProgressWriter("Writing Transpiled Scripts", buildTargets.Sum(bt => buildTracker.GetBuiltScripts(bt.Name).Count));
            //WTM:  Change:  Added:  Write QF first since some transpilation will be done while writing.
            //Types will be inferenced like TES4PublicanBloatedFloatOrmil, and if Standalone gets written first, those files will be incorrect.
            //The below OrderBy statement puts QF first.
            foreach (var buildTarget in buildTargets.OrderBy(bt => !bt.IsQF()))
            {
                buildTarget.Write(buildTracker, progressWriter);
            }
            progressWriter.WriteLast();
        }

        public void Transpile(bool writeTranspiled = true, int threadsNumber = DefaultThreads)
        {
            using (BuildTargetAdvancedCollection buildTargetsAdvanced = BuildTargetFactory.GetCollection(build, buildTargetsSimple))
            {
                BuildTracker buildTracker = new BuildTracker(BuildTargets);
                Transpile(build, buildTracker, buildTargetsAdvanced, threadsNumber);
                if (writeTranspiled)
                {
                    WriteTranspiled(buildTargetsAdvanced, buildTracker);
                }
            }
        }

        public static void PrepareWorkspace(Build build, IReadOnlyList<ITranspiledPathSource> buildTargets)
        {
            ProgressWriter preparingBuildWorkspaceProgressWriter = new ProgressWriter("Preparing Build Workspace", (buildTargets.Count * PrepareWorkspaceJob.CopyOperationsPerBuildTarget) + 1);
            PrepareWorkspaceJob prepareCommand = new PrepareWorkspaceJob(build, buildTargets);
            prepareCommand.Run(preparingBuildWorkspaceProgressWriter);
            preparingBuildWorkspaceProgressWriter.WriteLast();
        }

        public void Compile(IReadOnlyList<IBuildTargetForCompiler>? buildTargets = null, Action? afterPrepareWorkspace = null)
        {
            PrepareWorkspace(build, buildTargets != null ? buildTargets : BuildTargets);
            if (afterPrepareWorkspace != null) { afterPrepareWorkspace(); }
            CompileScriptJob task = new CompileScriptJob(build, buildTargets != null ? buildTargets : buildTargetsSimple);
            task.Run();
            Console.WriteLine("Build Complete");
        }
    }
}
