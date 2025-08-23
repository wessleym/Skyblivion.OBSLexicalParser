using Skyblivion.OBSLexicalParser.Builds;
using System;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class BuildWithComparisonCommand : LPCommand
    {
        public BuildWithComparisonCommand() : base("skyblivion:parser:buildwithcomparison", "Build with Comparison", "Transpile old, transpile new, and compare. Output to folder and compile.") { }

        public override void Execute()
        {
            BuildTargetExecutor? buildTargetExecutor = BuildTargetExecutor.TryConstruct();
            if (buildTargetExecutor == null) { return; }
            Console.WriteLine("""Enter the path to the "Data" directory (containing "Scripts" and "Source"):""");
            string? dataDirectory = Console.ReadLine();
            if (dataDirectory == null) { throw new InvalidOperationException(nameof(dataDirectory) + " was null."); }
            dataDirectory = dataDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var comparer = new BuildTargetComparer(buildTargetExecutor, dataDirectory);
            comparer.Run();
        }
    }
}
