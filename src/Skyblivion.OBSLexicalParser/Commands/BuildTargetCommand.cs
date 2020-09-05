using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class BuildTargetCommand : LPCommand
    {
        public const int DefaultThreads = 1;

        public BuildTargetCommand()
            : base("skyblivion:parser:build", "Build Target", "Create artifact(s) from OBScript source")
        {
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTargetFactory.DefaultNames));
            Input.AddArgument(new LPCommandArgument("threadsNumber", "Threads number", DefaultThreads.ToString()));
            Input.AddArgument(new LPCommandArgument("buildPath", "Build folder", Build.DEFAULT_BUILD_PATH));
        }

        public void Execute(IList<LPCommandArgument> input)
        {
            string targets = LPCommandArgumentOrOption.GetValue(input, "targets");
            int threadsNumber = int.Parse(LPCommandArgumentOrOption.GetValue(input, "threadsNumber"), CultureInfo.InvariantCulture);
            string buildPath = LPCommandArgumentOrOption.GetValue(input, "buildPath");
            Execute(targets, true, threadsNumber, buildPath);
        }
        public void Execute(bool writeTranspiledFilesAndCompile)
        {
            Execute(BuildTargetFactory.DefaultNames, writeTranspiledFilesAndCompile);
        }
        public override void Execute()
        {
            Execute(true);
        }

        public void Execute(string targets, bool writeTranspiledFilesAndCompile, int threadsNumber = DefaultThreads, string? buildPath = null)
        {
            if (!PreExecutionChecks(true, true, true, true)) { return; }
            if (buildPath == null) { buildPath = Build.DEFAULT_BUILD_PATH; }
            Build build = new Build(buildPath);
            BuildTarget[] buildTargets = BuildTargetFactory.ParseCollection(targets, build);
            if (writeTranspiledFilesAndCompile && !buildTargets.CanBuildAndWarnIfNot()) { return; }
            BuildTargetSimple[] buildTargetsSimple = BuildTargetFactory.GetCollection(buildTargets);
            using (BuildTargetAdvancedCollection buildTargetsAdvanced = BuildTargetFactory.GetCollection(build, buildTargetsSimple))
            {
                BuildTracker buildTracker = new BuildTracker(buildTargets);
                Transpile(build, buildTracker, buildTargetsAdvanced, threadsNumber);
                if (writeTranspiledFilesAndCompile)
                {
                    WriteTranspiled(buildTargetsAdvanced, buildTracker);
                }
            }
            if (writeTranspiledFilesAndCompile)
            {
                PrepareWorkspace(buildTargets);
                Compile(build, buildTargetsSimple);
                Console.WriteLine("Build Complete");
            }
        }

        private static void Transpile(Build build, BuildTracker buildTracker, BuildTargetAdvancedCollection buildTargets, int threadsNumber)
        {
            var buildPlan = buildTargets.GetBuildPlan(threadsNumber);
            ESMAnalyzer esmAnalyzer = buildTargets.ESMAnalyzer;
            int totalScripts = buildPlan.Sum(p => p.Value.Sum(chunk => chunk.Sum(c => c.Value.Count)));
            ProgressWriter progressWriter = new ProgressWriter("Transpiling Scripts", totalScripts);
            using (StreamWriter errorLog = new StreamWriter(build.GetErrorLogPath(), false))
            {
                foreach (var threadBuildPlan in buildPlan)
                {
                    TranspileChunkJob task = new TranspileChunkJob(buildTracker, threadBuildPlan.Value, esmAnalyzer);
                    task.RunTask(errorLog, progressWriter);
                }
            }
            progressWriter.WriteLast();
        }

        private static void WriteTranspiled(BuildTargetAdvancedCollection buildTargets, BuildTracker buildTracker)
        {
            ProgressWriter progressWriter = new ProgressWriter("Writing Transpiled Scripts", buildTargets.Sum(bt => buildTracker.GetBuiltScripts(bt.Name).Count));
            //WTM:  Added:  Change:  Transpile QF first since some transpilation will be done while writing.
            //Types will be inferenced like TES4PublicanBloatedFloatOrmil, and if Standalone gets written first, those files will be incorrect.
            //The below OrderBy statement puts QF first.
            foreach (var buildTarget in buildTargets.OrderBy(bt => !bt.IsQF()))
            {
                buildTarget.Write(buildTracker, progressWriter);
            }
            progressWriter.WriteLast();
        }

        private static void PrepareWorkspace(IList<BuildTarget> buildTargets)
        {
            ProgressWriter preparingBuildWorkspaceProgressWriter = new ProgressWriter("Preparing Build Workspace", buildTargets.Count * PrepareWorkspaceJob.CopyOperationsPerBuildTarget);
            PrepareWorkspaceJob prepareCommand = new PrepareWorkspaceJob(buildTargets);
            prepareCommand.Run(preparingBuildWorkspaceProgressWriter);
            preparingBuildWorkspaceProgressWriter.WriteLast();
        }

        private static void Compile(Build build, IList<BuildTargetSimple> buildTargets)
        {
            CompileScriptJob task = new CompileScriptJob(build, buildTargets);
            task.Run();
        }
    }
}