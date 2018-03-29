using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using System;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class BuildScriptCommand : LPCommand
    {
        public BuildScriptCommand()
            : base("skyblivion:parser:buildScript", "Build Script", "Create artifact from OBScript source")
        {
            Input.AddArgument(new LPCommandArgument("scriptName", "Script name"));
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTarget.DEFAULT_TARGETS));
            Input.AddArgument(new LPCommandArgument("buildPath", "Build folder", Build.DEFAULT_BUILD_PATH));
        }

        protected void execute(LPCommandInput input)
        {
            string scriptName = input.GetArgumentValue("scriptName");
            string targets = input.GetArgumentValue("targets");
            string buildPath = input.GetArgumentValue("buildPath");
            execute(scriptName, targets, buildPath);
        }

        public override void execute()
        {
            throw new NotImplementedException();
        }

        public void execute(string scriptName, string targets = BuildTarget.DEFAULT_TARGETS, string buildPath = null)
        {
            if (buildPath == null) { buildPath = Build.DEFAULT_BUILD_PATH; }
            //set_time_limit(60);
            Build build = new Build(buildPath);
            using (BuildLogServices buildLogServices = new BuildLogServices(build))
            {
                BuildTargetCollection buildTargets = BuildTargetFactory.getCollection(targets, build, buildLogServices);
                if (!buildTargets.canBuild())
                {
                    Console.WriteLine("Targets current build directory not clean.  Archive them manually, or run clean.sh.");
                    return;
                }

                TranspileScriptJob transpileJob = new TranspileScriptJob(buildTargets, scriptName);
#if !DEBUG
                try
                {
#endif
                    transpileJob.run();
#if !DEBUG
                }
                catch (ConversionException ex)
                {
                    Console.WriteLine("Exception occured.\r\n" + ex.GetType().FullName + ":  " + ex.Message);
                    return;
                }
#endif
                PrepareWorkspaceAndCompile(build, buildTargets);
            }
            Console.WriteLine("Build Complete");
            string compileLog = File.ReadAllText(build.getCompileStandardOutputPath());
            Console.WriteLine(compileLog);
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