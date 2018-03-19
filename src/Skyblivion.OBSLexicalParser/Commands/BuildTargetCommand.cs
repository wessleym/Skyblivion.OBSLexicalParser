using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.Extensions.StreamExtensions;
using Skyblivion.OBSLexicalParser.TES4.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands
{
    class BuildTargetCommand : LPCommand
    {
        protected BuildTargetCommand()
        {
            Name = "skyblivion:parser:build";
            Description = "Create artifact[s] from OBScript source";
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTarget.DEFAULT_TARGETS));
            Input.AddArgument(new LPCommandArgument("threadsNumber", "Threads number", "4"));
            Input.AddArgument(new LPCommandArgument("buildPath", "Build folder", Build.DEFAULT_BUILD_PATH));
        }

        protected void execute(List<LPCommandArgument> input)
        {
            set_time_limit(10800); // 3 hours is the maximum for this command. Need more? You really screwed something, full suite for all Oblivion vanilla data takes 20 minutes. :)
            try
            {
                string targets = input.Where(i => i.Name == "targets").First().Value;
                int threadsNumber = int.Parse(input.Where(i => i.Name == "threadsNumber").First().Value);
                string buildPath = input.Where(i => i.Name == "buildPath").First().Value;
                Build build = new Build(buildPath);
                BuildTargetCollection buildTargets = BuildTargetFactory.getCollection(targets, build);
                BuildTracker buildTracker = new BuildTracker(buildTargets);
                if (!buildTargets.canBuild())
                {
                    Console.WriteLine("Targets current build dir not clean, archive them manually or run ./clean.sh.");
                    return;
                }

                Console.WriteLine("Executing build...");
                ExecuteBuild(build, buildPath, buildTracker, buildTargets, threadsNumber);
                Console.WriteLine("Writing transpiled scripts..");
                foreach (var buildTarget in buildTargets)
                {
                    buildTarget.write(buildTracker);
                }

                //Hack - force ESM analyzer deallocation.
                ESMAnalyzer.deallocate();
                Console.WriteLine("Preparing build workspace...");
                /*
                 *
                 * @TODO - Create a factory that will provide a PrepareWorkspaceJob based on running system, so we can provide a
                 * native implementation for Windows
                 */
                PrepareWorkspaceJob prepareCommand = new PrepareWorkspaceJob(buildTargets);
                prepareCommand.run();
                Console.WriteLine("Workspace prepared...");
                CompileScriptJob task = new CompileScriptJob(buildTargets, build.getCompileLogPath());
                task.run();
                Console.WriteLine("Build completed.");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        private static void ExecuteBuild(Build build, string buildPath, BuildTracker buildTracker, BuildTargetCollection buildTargets, int threadsNumber)
        {
            // Create Metadata file if it doesn"t exist and clear
            string metadataPath = build.getBuildPath() + "Metadata";
            File.WriteAllText(metadataPath, "");
            var buildPlan = buildTargets.getBuildPlan(threadsNumber);
            using (FileStream errorLog = new FileStream(build.getErrorLogPath(), FileMode.Create))
            {
                foreach (var threadBuildPlan in buildPlan)
                {
                    /*
                     * We had some problems with sharing objects inside the jobs, so thats why we pass the path.
                     * Maybe later we can just inject Build and it will be nice and clean :)
                     */
                    TranspileChunkJob task = new TranspileChunkJob(buildTracker, buildPath, threadBuildPlan.Value);
                    try
                    {
                        task.runTask();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception occurred while transpiling:");
                        errorLog.WriteUTF8(ex.GetType().FullName + ":  " + ex.Message);
                    }
                }
            }
        }
    }
}