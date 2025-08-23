using Skyblivion.OBSLexicalParser.Builds;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class BuildTargetCommand : LPCommand
    {
        public BuildTargetCommand()
            : base("skyblivion:parser:build", "Build Target", "Create artifact(s) from OBScript source")
        {
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTargetFactory.DefaultNames));
            Input.AddArgument(new LPCommandArgument("threadsNumber", "Threads number", BuildTargetExecutor.DefaultThreads.ToString()));
            Input.AddArgument(new LPCommandArgument("buildPath", "Build folder", Build.DEFAULT_BUILD_PATH));
        }

        public static void Execute(IReadOnlyList<LPCommandArgument> input)
        {
            string targets = LPCommandArgumentOrOption.GetValue(input, "targets");
            int threadsNumber = int.Parse(LPCommandArgumentOrOption.GetValue(input, "threadsNumber"), CultureInfo.InvariantCulture);
            string buildPath = LPCommandArgumentOrOption.GetValue(input, "buildPath");
            Execute(targets, true, true, threadsNumber, buildPath);
        }
        internal static bool Execute(bool writeTranspiled, bool compile)
        {
            return Execute(BuildTargetFactory.DefaultNames, writeTranspiled, compile);
        }
        public override void Execute()
        {
            Execute(true, true);
        }

        private static bool Execute(string targets, bool writeTranspiled, bool compile, int threadsNumber = BuildTargetExecutor.DefaultThreads, string? buildPath = null)
        {
            if (compile && !writeTranspiled) { throw new ArgumentException(nameof(compile) + " must be false if " + nameof(writeTranspiled) + " is false.", nameof(compile)); }
            if (!PreExecutionChecks(true, true, true, true)) { return false; }
            BuildTargetExecutor? buildTargetExecutor = BuildTargetExecutor.TryConstruct(targets, buildPath: buildPath);
            if (buildTargetExecutor == null) { return false; }
            buildTargetExecutor.Transpile(writeTranspiled: writeTranspiled, threadsNumber: threadsNumber);
            if (compile)
            {
                buildTargetExecutor.Compile();
            }
            return true;
        }
    }
}