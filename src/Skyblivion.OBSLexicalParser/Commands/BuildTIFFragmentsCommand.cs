using Dissect.Lexer.TokenStream;
using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Input;
using Skyblivion.OBSLexicalParser.TES4.AST.VariableDeclaration;
using Skyblivion.OBSLexicalParser.TES4.Lexer;
using Skyblivion.OBSLexicalParser.TES4.Parsers;
using Skyblivion.OBSLexicalParser.TES5.AST;
using Skyblivion.OBSLexicalParser.TES5.AST.Scope;
using Skyblivion.OBSLexicalParser.TES5.Converter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Skyblivion.OBSLexicalParser.Commands
{
    class BuildTIFFragmentsCommand : LPCommand
    {
        private FragmentsReferencesBuilder fragmentsReferencesBuilder;
        public BuildTIFFragmentsCommand()
        {
            this.fragmentsReferencesBuilder = new FragmentsReferencesBuilder();
        }

        protected void configure()
        {
            Name = "skyblivion:parser:buildTifFragments";
            Description = "Run lexing and parsing test against whole TIF fragments suite and build papyrus scripts";
            Input.AddArgument(new LPCommandArgument("buildPath", "Build folder", Build.DEFAULT_BUILD_PATH));
            Input.AddOption(new LPCommandOption("skip-parsing", "sp", "Skip the parsing part.", "false"));
            Input.AddOption(new LPCommandOption("mode", null, "The mode this build runs in. Allowed: sloppy (will start building on over 50% scripts parsed), normal (85% and over will trigger the build), strict (over 95% will trigger the build), and perfect (only 100% will trigger the build), defaults to strict.", "strict"));
        }

        protected void execute(LPCommandInput input)
        {
            set_time_limit(10800); // 3 hours is the maximum for this command. Need more? You really screwed something, full suite for all Oblivion vanilla data takes 20 minutes. :)
            string mode = input.GetArgumentValue("mode");
            float threshold;
            switch (mode)
            {
                case "sloppy":
                    {
                        threshold = 0.5f;
                        break;
                    }
                case "normal":
                    {
                        threshold = 0.85f;
                        break;
                    }
                case "strict":
                default:
                    {
                        threshold = 0.95f;
                        break;
                    }
                case "perfect":
                    {
                        threshold = 1;
                        break;
                    }
            }

            bool skipParsing = input.GetOptionBoolean("skip-parsing");
            if (!skipParsing)
            {
                SyntaxErrorCleanParser parser = new SyntaxErrorCleanParser(new TES4ObscriptCodeGrammar());
                //parser = new Parser(new TES4OBScriptGrammar());
                string buildPath = input.GetArgumentValue("buildPath");
                TES4ToTES5ASTTIFFragmentConverter converter = TES4ToTES5ASTTIFFragmentConverterFactory.GetConverter(new Build(buildPath));
                string inputFolder = "./Fragments/TIF/fragments/";
                string outputFolder = "./Fragments/TIF/PapyrusFragments/";
                string[] scandir = Directory.GetFiles(inputFolder);
                int success = 0, total = 0;
                Dictionary<string, TES5MultipleScriptsScope> ASTTable = new Dictionary<string, TES5MultipleScriptsScope>();
                Console.WriteLine("Lexing and parsing..");
                int totalNumber = scandir.Length;
                foreach (var scriptPath in scandir)
                {
                    if (!scriptPath.EndsWith(".txt"))
                    {
                        continue;
                    }

                    if ((total % 10) == 0)
                    {
                        Console.WriteLine(total + "/" + totalNumber + "...");
                    }

                    string outputScriptPath = scriptPath.Substring(0, scriptPath.Length - 4)+".psc";
                    string path = inputFolder + scriptPath;
                    total++;
                    try
                    {
                        string scriptName = scriptPath.Substring(0, scriptPath.Length - 4);
                        Console.WriteLine(scriptName + " ...");
                        FragmentLexer lexer = new FragmentLexer();
                        ArrayTokenStream tokens = lexer.lex(File.ReadAllText(path));
                        TES4VariableDeclarationList variableList = this.fragmentsReferencesBuilder.buildVariableDeclarationList(inputFolder + scriptName + ".references");
                        TES5MultipleScriptsScope AST = parser.ParseAs<TES5MultipleScriptsScope>(tokens);
                        ASTTable[scriptPath] = AST;
                        TES5Target TES5AST = converter.convert(scriptName, variableList, AST);
                        string outputScript = TES5AST.output();
                        File.WriteAllText(outputFolder + outputScriptPath, outputScript);
                        Process.Start("lua", "\"Utilities/beautifier.lua\" \"" + outputFolder + outputScriptPath + "\"");
                        success++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(scriptPath + "\r\n" + e.GetType().FullName + "\r\n" + e.Message + "\r\n");
                        continue;
                    }
                }

                float successRate = (float)success / total;
                if (successRate < threshold)
                {
                    float percent = (float)Math.Round(successRate * 100);
                    Console.WriteLine("ERROR: Build failed on parsing step in " + mode + " mode. The rate is " + success + "/" + total + " ( " + percent + " %)");
                    return;
                }

                Console.WriteLine("Parsing in " + mode + " mode succedeed (rate " + success + "/" + total + ").  Copying Skyrim scripts and parsed papyrus fragments to build folder...");
            }

            Console.WriteLine("Build in " + mode + " mode succeeded!");
        }
    }
}