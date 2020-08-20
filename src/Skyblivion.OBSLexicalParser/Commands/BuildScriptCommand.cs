using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.TES4.Context;
using Skyblivion.OBSLexicalParser.TES5.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
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
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTargetFactory.DefaultNames));
            Input.AddArgument(new LPCommandArgument("buildPath", "Build folder", Build.DEFAULT_BUILD_PATH));
        }

        protected void Execute(LPCommandInput input)
        {
            string scriptName = input.GetArgumentValue("scriptName");
            string targets = input.GetArgumentValue("targets");
            string buildPath = input.GetArgumentValue("buildPath");
            Execute(scriptName, targets, buildPath);
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public void Execute(string scriptName, string targets = BuildTargetFactory.DefaultNames, string? buildPath = null)
        {
            if (!PreExecutionChecks(true, true, true, true)) { return; }
            if (buildPath == null) { buildPath = Build.DEFAULT_BUILD_PATH; }
            Build build = new Build(buildPath);
            BuildTarget[] buildTargets = BuildTargetFactory.ParseCollection(targets, build);
            if (!buildTargets.CanBuildAndWarnIfNot()) { return; }
            BuildTargetSimple[] buildTargetsSimple = BuildTargetFactory.GetCollection(buildTargets);
            using (BuildLogServiceCollection buildLogServices = BuildLogServiceCollection.DeleteAndStartNewFiles(build))
            {
                ESMAnalyzer esmAnalyzer;
                BuildTargetAdvancedCollection buildTargetsAdvanced = BuildTargetFactory.GetCollection(buildTargetsSimple, buildLogServices, out esmAnalyzer, out _);
                using (esmAnalyzer)
                {
                    TranspileScriptJob transpileJob = new TranspileScriptJob(buildTargetsAdvanced, scriptName, esmAnalyzer);
#if !DEBUG
                    try
                    {
#endif
                        transpileJob.Run();
#if !DEBUG
                    }
                    catch (ConversionException ex)
                    {
                        Console.WriteLine("Exception occured." + Environment.NewLine + ex.GetType().FullName + ":  " + ex.Message);
                        return;
                    }
#endif
                }
            }
            PrepareWorkspace(buildTargets);
            Compile(build, buildTargetsSimple);
            Console.WriteLine("Build Complete");
            string compileLog = File.ReadAllText(build.GetCompileStandardOutputPath());
            Console.WriteLine(compileLog);
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