using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            //set_time_limit(10800); // 3 hours is the maximum for this command. Need more? You really screwed something, full suite for all Oblivion vanilla data takes 20 minutes. :)
            Build build = new Build(buildPath);
            using (BuildLogServices buildLogServices = new BuildLogServices(build))
            {
                BuildTargetCollection buildTargets = BuildTargetFactory.getCollection(targets, build, buildLogServices);
                BuildTracker buildTracker = new BuildTracker(buildTargets);
                if (!buildTargets.canBuild())
                {
                    Console.WriteLine("Targets current build directory not clean.  Archive them manually, or run clean.sh.");
                    return;
                }

                ExecuteBuild(build, buildPath, buildTracker, buildTargets, threadsNumber);
                ProgressWriter writingTranspiledScriptsProgressWriter = new ProgressWriter("Writing Transpiled Scripts", buildTargets.Sum(bt=>bt.getSourceFileList().Count()));
                foreach (var buildTarget in buildTargets)
                {
                    buildTarget.write(buildTracker, writingTranspiledScriptsProgressWriter);
                }
                writingTranspiledScriptsProgressWriter.WriteLast();

                //Hack - force ESM analyzer deallocation.
                ESMAnalyzer.deallocate();
                PrepareWorkspaceAndCompile(build, buildTargets);
            }
            Console.WriteLine("Build Complete");
        }

        private static void ExecuteBuild(Build build, string buildPath, BuildTracker buildTracker, BuildTargetCollection buildTargets, int threadsNumber)
        {
            // Create Metadata file if it doesn"t exist and clear
            MetadataLogService.ClearFile(build);
            var buildPlan = buildTargets.getBuildPlan(threadsNumber);
            int totalScripts = buildPlan.Sum(p => p.Value.Sum(chunk => chunk.Sum(c => c.Value.Count)));
            ProgressWriter progressWriter = new ProgressWriter("Transpiling Scripts", totalScripts);
            using (FileStream errorLog = new FileStream(build.getErrorLogPath(), FileMode.Create))
            {
                foreach (var threadBuildPlan in buildPlan)
                {
                    /*
                     * We had some problems with sharing objects inside the jobs, so thats why we pass the path.
                     * Maybe later we can just inject Build and it will be nice and clean :)
                     */
                    using (TranspileChunkJob task = new TranspileChunkJob(buildTracker, buildPath, threadBuildPlan.Value))
                    {
                        task.runTask(errorLog, progressWriter);
                    }
                }
            }
            progressWriter.WriteLast();
        }

        private static void PrepareWorkspaceAndCompile(Build build, BuildTargetCollection buildTargets)
        {
            ProgressWriter preparingBuildWorkspaceProgressWriter = new ProgressWriter("Preparing Build Workspace", buildTargets.Count() * PrepareWorkspaceJob.CopyOperationsPerBuildTarget);
            PrepareWorkspaceJob prepareCommand = new PrepareWorkspaceJob(buildTargets);
            prepareCommand.run(preparingBuildWorkspaceProgressWriter);
            preparingBuildWorkspaceProgressWriter.WriteLast();
            CompileScriptJob task = new CompileScriptJob(build, buildTargets);
            task.run();
        }
    }
}