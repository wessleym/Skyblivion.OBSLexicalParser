using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands
{
    internal class BuildSourceScriptsFolderCommand : LPCommand
    {
        private static readonly string targetDirectorySegment = "Source" + Path.DirectorySeparatorChar + "Scripts" + Path.DirectorySeparatorChar;
        public BuildSourceScriptsFolderCommand() : base("skyblivion:parser:buildSourceScriptsFolderScripts", "Build Source Scripts Folder", "Build " + targetDirectorySegment + " folder") { }

        public override void Execute()
        {
            string folderPath = DataDirectory.BuildSourceScriptsFolderDirectoryPathBase;
            string importPath = folderPath + "Import" + Path.DirectorySeparatorChar;
            string outputPath = folderPath + "Output" + Path.DirectorySeparatorChar;
            BuildTargetExecutor? buildTargetExecutor = BuildTargetExecutor.TryConstruct();
            if (buildTargetExecutor == null) { return; }
            Console.WriteLine($"""Enter the path to the "{targetDirectorySegment}" directory:""");
            string? sourceDirectory = Console.ReadLine();
            if (sourceDirectory == null) { throw new InvalidOperationException(nameof(sourceDirectory) + " was null."); }
            sourceDirectory = sourceDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            Console.WriteLine("Creating Directories...");
            FileSystemEntryUtility.CreateDirectoryEmpty(folderPath);
            Directory.CreateDirectory(importPath);
            Directory.CreateDirectory(outputPath);
            Build build = new Build();
            string dependenciesPath = build.GetDependenciesPath();
            FileSystemEntryUtility.CopyDirectoryFiles(dependenciesPath, importPath, false, progressWriterLabel: "Copying Dependencies to Import Folder");
            FileSystemEntryUtility.CopyDirectoryFiles(sourceDirectory, importPath, true, progressWriterLabel: "Copying Source to Import Folder");
            PapyrusCompiler.Run(sourceDirectory, importPath, outputPath, folderPath);
            Console.WriteLine("Deleting " + folderPath + "...");
            Directory.Delete(folderPath, true);
        }
    }
}
