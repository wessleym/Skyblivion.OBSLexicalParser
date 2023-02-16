using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Builds.TIF;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.TES4.Context;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        public void Execute(IReadOnlyList<LPCommandArgument> input)
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

        private static void Temp()
        {
            System.Text.RegularExpressions.Regex commentRE = new System.Text.RegularExpressions.Regex(";([^\r\n]+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            var comments = Directory.EnumerateFiles(@"C:\Users\Wess\Documents\Visual Studio 2022\Projects\Skyblivion.OBSLexicalParser\Skyblivion.OBSLexicalParserApp\bin\Debug\net6.0\Data\BuildTargets", "*", SearchOption.AllDirectories).Select(f => new { File = Path.GetFileName(f), Comments = commentRE.Matches(File.ReadAllText(f)).Cast<Match>().Select(m => m.Groups[1].Value.Trim()).ToArray() }).Where(x => x.Comments.Any()).ToArray();
            string allTranspiled = string.Join("\r\n", Directory.EnumerateFiles(@"C:\Users\Wess\Documents\Visual Studio 2022\Projects\Skyblivion.OBSLexicalParser\Skyblivion.OBSLexicalParserApp\bin\Debug\net6.0\Data\Build", "*", SearchOption.AllDirectories).Select(f => File.ReadAllText(f)));
            var missingComments = comments.Select(x => new { File = x.File, Comments = x.Comments.Where(c => !allTranspiled.Contains(c)).ToArray() }).Where(x=>x.Comments.Any()).ToArray();
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
                PrepareWorkspace(build, buildTargets);
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
            const string generating = "Generating INFO AddTopic Scripts...";
            progressWriter.Write(generating);
            AddTopicBuilderCommand.GenerateINFOAddTopicScripts(buildTargets.ESMAnalyzer, buildTracker, buildTargets.First(t => t.IsTIF()));
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

        private static void PrepareWorkspace(Build build, IReadOnlyList<BuildTarget> buildTargets)
        {
            ProgressWriter preparingBuildWorkspaceProgressWriter = new ProgressWriter("Preparing Build Workspace", (buildTargets.Count * PrepareWorkspaceJob.CopyOperationsPerBuildTarget)+1);
            PrepareWorkspaceJob prepareCommand = new PrepareWorkspaceJob(build, buildTargets);
            prepareCommand.Run(preparingBuildWorkspaceProgressWriter);
            preparingBuildWorkspaceProgressWriter.WriteLast();
        }

        private static void Compile(Build build, IReadOnlyList<BuildTargetSimple> buildTargets)
        {
            CompileScriptJob task = new CompileScriptJob(build, buildTargets);
            task.Run();
        }
    }
}