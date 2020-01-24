using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Factory;
using Skyblivion.OBSLexicalParser.TES5.Service;
using Skyblivion.OBSLexicalParser.TES5.Types;
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
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTarget.DEFAULT_TARGETS));
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
            Execute(BuildTarget.DEFAULT_TARGETS, writeTranspiledFilesAndCompile);
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
            using (BuildLogServices buildLogServices = new BuildLogServices(build))
            {
                ESMAnalyzer esmAnalyzer;
                TES5InheritanceGraphAnalyzer inheritanceGraphAnalyzer;
                TES5TypeInferencer typeInferencer;
                BuildTargetCollection buildTargets = BuildTargetFactory.GetCollection(targets, build, buildLogServices, false, out esmAnalyzer, out inheritanceGraphAnalyzer, out typeInferencer);
                if (writeTranspiledFilesAndCompile && !buildTargets.CanBuildAndWarnIfNot()) { return; }
                BuildTracker buildTracker = new BuildTracker(buildTargets);
                TES5StaticGlobalScopesFactory staticGlobalScopesFactory = new TES5StaticGlobalScopesFactory(esmAnalyzer);
                Transpile(build, buildTracker, buildTargets, buildLogServices, threadsNumber, esmAnalyzer, staticGlobalScopesFactory, inheritanceGraphAnalyzer, typeInferencer);
                if (writeTranspiledFilesAndCompile)
                {
                    WriteTranspiled(buildTargets, buildTracker);
                }
                esmAnalyzer.Deallocate();//Hack - force ESM analyzer deallocation.
                if (writeTranspiledFilesAndCompile)
                {
                    PrepareWorkspace(buildTargets);
                    Compile(build, buildTargets);
                }
            }
            if (writeTranspiledFilesAndCompile)
            {
                Console.WriteLine("Build Complete");
            }
        }

        private static void Transpile(Build build, BuildTracker buildTracker, BuildTargetCollection buildTargets, BuildLogServices buildLogServices, int threadsNumber, ESMAnalyzer esmAnalyzer, TES5StaticGlobalScopesFactory staticGlobalScopesFactory, TES5InheritanceGraphAnalyzer inheritanceGraphAnalyzer, TES5TypeInferencer typeInferencer)
        {
            var buildPlan = buildTargets.GetBuildPlan(threadsNumber);
            int totalScripts = buildPlan.Sum(p => p.Value.Sum(chunk => chunk.Sum(c => c.Value.Count)));
            ProgressWriter progressWriter = new ProgressWriter("Transpiling Scripts", totalScripts);
            using (StreamWriter errorLog = new StreamWriter(build.GetErrorLogPath(), false))
            {
                foreach (var threadBuildPlan in buildPlan)
                {
                    TranspileChunkJob task = new TranspileChunkJob(build, buildTracker, buildLogServices, threadBuildPlan.Value, esmAnalyzer, staticGlobalScopesFactory, inheritanceGraphAnalyzer, typeInferencer);
                    task.RunTask(errorLog, progressWriter);
                }
            }
            progressWriter.WriteLast();
        }

        private static void WriteTranspiled(BuildTargetCollection buildTargets, BuildTracker buildTracker)
        {
            ProgressWriter progressWriter = new ProgressWriter("Writing Transpiled Scripts", buildTargets.Sum(bt => buildTracker.GetBuiltScripts(bt.GetTargetName()).Count));
            //WTM:  Note:  QF's progress will be underestimated since buildTracker.GetBuiltScripts(bt.GetTargetName()).Count is greater than the actual number of output scripts.
            foreach (var buildTarget in buildTargets)
            {
                buildTarget.Write(buildTracker, progressWriter);
            }
            progressWriter.WriteLast();
        }

        private static void PrepareWorkspace(BuildTargetCollection buildTargets)
        {
            ProgressWriter preparingBuildWorkspaceProgressWriter = new ProgressWriter("Preparing Build Workspace", buildTargets.Count * PrepareWorkspaceJob.CopyOperationsPerBuildTarget);
            PrepareWorkspaceJob prepareCommand = new PrepareWorkspaceJob(buildTargets);
            prepareCommand.Run(preparingBuildWorkspaceProgressWriter);
            preparingBuildWorkspaceProgressWriter.WriteLast();
        }

        private static void Compile(Build build, BuildTargetCollection buildTargets)
        {
            CompileScriptJob task = new CompileScriptJob(build, buildTargets);
            task.Run();
        }
    }
}