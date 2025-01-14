using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Data;
using Skyblivion.OBSLexicalParser.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands
{
    internal class BuildModifiedScriptsCommand : LPCommand
    {
        public BuildModifiedScriptsCommand()
            : base("skyblivion:parser:buildModifiedScripts", "Build Modified Scripts", "Build standard scripts modified by Skyblivion Team")
        { }

        public override void Execute()
        {
            if (!PreExecutionChecks(false, false, false, true)) { return; }
            string inputsPath = DataDirectory.ModifiedScriptsInputsDirectoryPath;
            string preprocessedPath = DataDirectory.ModifiedScriptsPreprocessedDirectoryPath;
            string skseSourcePath = DataDirectory.ModifiedScriptsSKSESourceDirectoryPath;
            string importGeneratedPath = DataDirectory.ModifiedScriptsImportGeneratedDirectoryPath;
            Console.WriteLine($"""
Please place the following files:

{inputsPath}
The scripts copied from our Git repository/Skyblivion.OBSLexicalParserApp/ModifiedScripts_Input

{skseSourcePath}
The SKSE source for which the files should be compiled (for example, from skse64_X.7z\skse64_X\Data\Scripts\Source)
""");
            Console.WriteLine("Please any key to continue.");
            Console.ReadKey(true);
            Console.WriteLine("Preparing...");
            Build build = new Build();
            string dependenciesPath = build.GetDependenciesPath();
            foreach (string path in new string[] { inputsPath, skseSourcePath, dependenciesPath })
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("The directory " + path + " did not exist.");
                    return;
                }
            }
            string outputsDirectoryPath = DataDirectory.ModifiedScriptsOutputsDirectoryPath;
            foreach (string directoryPath in new string[] { preprocessedPath, importGeneratedPath, outputsDirectoryPath })
            {
                try { Directory.Delete(directoryPath, true); } catch (DirectoryNotFoundException) { }
                Directory.CreateDirectory(preprocessedPath);
            }
            FileWriter.CopyDirectoryFiles(skseSourcePath, importGeneratedPath, false);
            foreach (string inputPath in Directory.EnumerateFiles(inputsPath, "*.psc", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(inputPath);
                string directoryName = Path.GetFileName(Path.GetDirectoryName(inputPath))!;
                string inputContents = File.ReadAllText(inputPath);
                bool amendment = directoryName == "Amendments";//This system is no longer used since all of our scripts are now originals.
                string preprocessedFilePath = preprocessedPath + fileName;
                string importFilePath = importGeneratedPath + fileName;
                if (amendment)
                {
                    string outputContents = File.ReadAllText(skseSourcePath + fileName) + "\r\n\r\n; Skyblivion Amendments:\r\n\r\n" + inputContents;
                    File.WriteAllText(preprocessedFilePath, outputContents);
                    File.WriteAllText(importFilePath, outputContents);
                }
                else
                {
                    FileWriter.WriteAllTextOrThrowIfExists(preprocessedFilePath, inputContents);
                    FileWriter.WriteAllTextOrThrowIfExists(importFilePath, inputContents);
                }
            }
            //I am copying all dependent files I've witnessed as being needed. Without them, compilation gives errors.
            //Sometimes, SKSE has already provided these files, depending on the edition.
            foreach (string fileNameNoExt in new string[] { "AssociationType", "Class", "Debug", "EffectShader", "EncounterZone", "Explosion", "GlobalVariable", "Idle", "ImageSpaceModifier", "ImpactDataset", "Key", "Light", "Location", "LocationRefType", "MiscObject", "Package", "Projectile", "ReferenceAlias", "Scene", "Static", "Topic", "VoiceType", "WordOfPower", "WorldSpace" })
            {
                string fileName = fileNameNoExt + ".psc";
                try
                {
                    File.Copy(dependenciesPath + fileName, importGeneratedPath + fileName);
                }
                catch (IOException ex) when (ex.Message.StartsWith("The file '") && ex.Message.EndsWith("' already exists.") &&
                    //Different versions of SKSE modify files other editions do not.
                    (
                    fileNameNoExt == "Location" ||//Special Edition
                    fileNameNoExt == "Scene" //VR
                    ))
                { }
            }
            IEnumerable<string>? importFilePaths = null;
            try
            {
                importFilePaths = Directory.EnumerateFiles(DataDirectory.ModifiedScriptsImportDirectoryPath);
            }
            catch (DirectoryNotFoundException) { }
            if (importFilePaths != null)
            {
                foreach (string importPath in importFilePaths)
                {
                    string importGeneratedFilePath = DataDirectory.ModifiedScriptsImportGeneratedDirectoryPath + Path.GetFileName(importPath);
                    try { File.Copy(importPath, importGeneratedFilePath, false); }
                    catch (IOException) { }
                }
            }
            string outputDirectoryBase = DataDirectory.ModifiedScriptsDirectoryPathBase;
            string outputPath = outputDirectoryBase + "Output.txt";
            string errorPath = outputDirectoryBase + "Error.txt";
            File.Delete(outputPath);
            File.Delete(errorPath);
            PapyrusCompiler.Run(preprocessedPath, importGeneratedPath, outputsDirectoryPath, outputPath, errorPath);
            if (File.ReadAllText(errorPath) != "")
            {
                Console.WriteLine(errorPath + " contained errors. Please check this file.");
            }
        }
    }
}
