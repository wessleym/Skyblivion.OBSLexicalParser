using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class BuildTargetCommand : LPCommand
    {
        public const int DefaultThreads = 4;

        public BuildTargetCommand()
            : base("skyblivion:parser:build", "Build Target", "Create artifact(s) from OBScript source")
        {
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTarget.DEFAULT_TARGETS));
            Input.AddArgument(new LPCommandArgument("threadsNumber", "Threads number", DefaultThreads.ToString()));
            Input.AddArgument(new LPCommandArgument("buildPath", "Build folder", Build.DEFAULT_BUILD_PATH));
        }

        public void execute(List<LPCommandArgument> input)
        {
            string targets = input.Where(i => i.Name == "targets").First().Value;
            int threadsNumber = int.Parse(input.Where(i => i.Name == "threadsNumber").First().Value);
            string buildPath = input.Where(i => i.Name == "buildPath").First().Value;
            execute(targets, threadsNumber, buildPath);
        }

        public override void execute()
        {
            execute(BuildTarget.DEFAULT_TARGETS);
        }

        public void execute(string targets, int threadsNumber = DefaultThreads, string buildPath = null)
        {
            if (buildPath == null) { buildPath = Build.DEFAULT_BUILD_PATH; }
            Build build = new Build(buildPath);
            using (BuildLogServices buildLogServices = new BuildLogServices(build))
            {
                BuildTargetCollection buildTargets = BuildTargetFactory.getCollection(targets, build, buildLogServices);
                if (!buildTargets.canBuild())
                {
                    Console.WriteLine("Targets current build directory not clean.  Archive them manually, or run clean.sh.");
                    return;
                }
                BuildTracker buildTracker = new BuildTracker(buildTargets);
                Transpile(build, buildTracker, buildTargets, buildLogServices, threadsNumber);
                WriteTranspiled(buildTargets, buildTracker);
                ESMAnalyzer.deallocate();//Hack - force ESM analyzer deallocation.
                PrepareWorkspace(build, buildTargets);
                Compile(build, buildTargets);
            }
            Console.WriteLine("Build Complete");
        }

        private static void Transpile(Build build, BuildTracker buildTracker, BuildTargetCollection buildTargets, BuildLogServices buildLogServices, int threadsNumber)
        {
            MetadataLogService.DeleteFile(build);
            var buildPlan = buildTargets.getBuildPlan(threadsNumber);
            int totalScripts = buildPlan.Sum(p => p.Value.Sum(chunk => chunk.Sum(c => c.Value.Count)));
            ProgressWriter progressWriter = new ProgressWriter("Transpiling Scripts", totalScripts);
            using (FileStream errorLog = new FileStream(build.getErrorLogPath(), FileMode.Create))
            {
                foreach (var threadBuildPlan in buildPlan)
                {
                    TranspileChunkJob task = new TranspileChunkJob(build, buildTracker, buildLogServices, threadBuildPlan.Value);
                    task.runTask(errorLog, progressWriter);
                }
            }
            progressWriter.WriteLast();
        }

        private static void WriteTranspiled(BuildTargetCollection buildTargets, BuildTracker buildTracker)
        {
            ProgressWriter writingTranspiledScriptsProgressWriter = new ProgressWriter("Writing Transpiled Scripts", buildTargets.Sum(bt => bt.getSourceFileList().Count()));
            foreach (var buildTarget in buildTargets)
            {
                buildTarget.write(buildTracker, writingTranspiledScriptsProgressWriter);
            }
            writingTranspiledScriptsProgressWriter.WriteLast();
        }

        private static void PrepareWorkspace(Build build, BuildTargetCollection buildTargets)
        {
            ProgressWriter preparingBuildWorkspaceProgressWriter = new ProgressWriter("Preparing Build Workspace", buildTargets.Count() * PrepareWorkspaceJob.CopyOperationsPerBuildTarget);
            PrepareWorkspaceJob prepareCommand = new PrepareWorkspaceJob(buildTargets);
            prepareCommand.run(preparingBuildWorkspaceProgressWriter);
            preparingBuildWorkspaceProgressWriter.WriteLast();
        }

        private static void Compile(Build build, BuildTargetCollection buildTargets)
        {
            CompileScriptJob task = new CompileScriptJob(build, buildTargets);
            task.run();
        }
    }
}